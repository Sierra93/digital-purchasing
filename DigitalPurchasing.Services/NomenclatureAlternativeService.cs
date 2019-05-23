using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using EFCore.BulkExtensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Services
{
    public sealed class NomenclatureAlternativeService : INomenclatureAlternativeService
    {
        private readonly ApplicationDbContext _db;

        public NomenclatureAlternativeService(
            ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        public void UpdateAlternative(NomenclatureAlternativeVm model)
        {
            var entity = _db.NomenclatureAlternatives.Find(model.Id);

            //entity.ClientName = model.ClientName;
            //entity.ClientType = model.ClientType;
            entity.Name = model.Name;
            entity.Code = model.Code;

            entity.ResourceUomId = model.ResourceUomId;
            entity.ResourceUomValue = model.ResourceUomValue;

            entity.BatchUomId = model.BatchUomId;
            entity.ResourceBatchUomId = model.ResourceBatchUomId;

            entity.MassUomId = model.MassUomId;
            entity.MassUomValue = model.MassUomValue;

            entity.PackUomId = model.PackUomId;
            entity.PackUomValue = model.PackUomValue;

            _db.SaveChanges();
        }

        public NomenclatureAlternativeVm FindBestFuzzyMatch(Guid ownerId, string nomName, int maxNameDistance)
        {
            var results = from item in _db.NomenclatureAlternatives.IgnoreQueryFilters()
                          let distance = ApplicationDbContext.LevenshteinDistanceFunc(nomName, item.Name, maxNameDistance)
                          where item.OwnerId == ownerId &&
                                distance.HasValue
                          orderby distance
                          select item;

            return results.FirstOrDefault()?.Adapt<NomenclatureAlternativeVm>();
        }

        public NomenclatureAlternativeVm GetAlternativeById(Guid id)
        {
            var entity = _db.NomenclatureAlternatives.Find(id);
            var result = entity.Adapt<NomenclatureAlternativeVm>();
            var link = _db.NomenclatureAlternativeLinks
                .Include(q => q.Supplier)
                .Include(q => q.Customer)
                .FirstOrDefault(q => q.AlternativeId == id);
            if (link != null)
            {
                result.ClientName = link.CustomerId.HasValue
                    ? link.Customer.Name
                    : link.Supplier.Name;
                result.ClientType = link.CustomerId.HasValue
                    ? ClientType.Customer
                    : ClientType.Supplier;
            }

            return result;
        }

        public void AddNomenclatureForCustomer(Guid prItemId)
        {
            var prItem = _db.PurchaseRequestItems.Include(q => q.PurchaseRequest).First(q => q.Id == prItemId);
            if (prItem.NomenclatureId.HasValue && prItem.PurchaseRequest.CustomerId.HasValue)
            {
                AddOrUpdateNomenclatureAlts(
                    prItem.PurchaseRequest.OwnerId,
                    prItem.PurchaseRequest.CustomerId.Value,
                    ClientType.Customer,
                    prItem.NomenclatureId.Value,
                    prItem.RawName,
                    prItem.RawCode,
                    prItem.RawUomMatchId);
            }
        }

        public void AddNomenclatureForSupplier(Guid soItemId)
        {
            var soItem = _db.SupplierOfferItems.IgnoreQueryFilters().Include(q => q.SupplierOffer).First(q => q.Id == soItemId);
            if (soItem.NomenclatureId.HasValue && soItem.SupplierOffer.SupplierId.HasValue)
            {
                AddOrUpdateNomenclatureAlts(
                    soItem.SupplierOffer.OwnerId,
                    soItem.SupplierOffer.SupplierId.Value,
                    ClientType.Supplier,
                    soItem.NomenclatureId.Value,
                    soItem.RawName,
                    soItem.RawCode,
                    soItem.RawUomId);
            }
        }

        public void AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            Guid nomenclatureId, string name, string code, Guid? batchUomId)
            => AddOrUpdateNomenclatureAlts(ownerId, clientId, clientType,
                new List<AddOrUpdateAltDto>
                {
                    new AddOrUpdateAltDto() { NomenclatureId = nomenclatureId, Name = name, Code = code, BatchUomId = batchUomId }
                });

        // todo: add owner id?
        private void AddAlternative(Guid nomenclatureId, Guid clientId, ClientType clientType, string name, string code, Guid? uom)
        {
            name = name.Trim();
            code = code.Trim();

            var altNamesQry = _db.NomenclatureAlternatives
                .Include(q => q.Link)
                .AsQueryable();

            altNamesQry = clientType == ClientType.Customer
                ? altNamesQry.Where(q => q.Link.CustomerId == clientId)
                : altNamesQry.Where(q => q.Link.SupplierId == clientId);

            var altName = altNamesQry.FirstOrDefault(q =>
                q.NomenclatureId == nomenclatureId && q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (altName != null)
            {
                if (string.IsNullOrEmpty(altName.Code))
                    altName.Code = code;
                if (!altName.BatchUomId.HasValue)
                {
                    altName.BatchUomId = uom;
                }
            }
            else
            {
                _db.NomenclatureAlternatives.Add(new NomenclatureAlternative
                {
                    Name = name,
                    Code = code,
                    BatchUomId = uom,
                    NomenclatureId = nomenclatureId,
                    Link = new NomenclatureAlternativeLink
                    {
                        CustomerId = clientType == ClientType.Customer ? clientId : (Guid?)null,
                        SupplierId = clientType == ClientType.Supplier ? clientId : (Guid?)null
                    }
                });
            }

            _db.SaveChanges();
        }

        public void AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            List<AddOrUpdateAltDto> alts)
        {
            if (!alts.Any())
            {
                return;
            }

            var altNomenclaturesQry = _db.NomenclatureAlternatives
                .IgnoreQueryFilters()
                .Where(q => q.OwnerId == ownerId);

            altNomenclaturesQry = clientType == ClientType.Customer
                ? altNomenclaturesQry.Where(q => q.Link.CustomerId == clientId)
                : altNomenclaturesQry.Where(q => q.Link.SupplierId == clientId);

            var altNomenclatures = altNomenclaturesQry.ToList();

            var forBulkInsertNA = new List<NomenclatureAlternative>();
            var forBulkUpdateNA = new List<NomenclatureAlternative>();
            var forBulkInsertLink = new List<NomenclatureAlternativeLink>();

            foreach (var alt in alts)
            {
                string altNomName = alt.Name?.Trim();
                var altByName = altNomenclatures.FirstOrDefault(q =>
                    q.NomenclatureId == alt.NomenclatureId &&
                    q.Name.Equals(altNomName, StringComparison.InvariantCultureIgnoreCase));

                if (altByName != null)
                {
                    bool changed = false;

                    if (altByName.Code != alt.Code?.Trim())
                    {
                        altByName.Code = alt.Code?.Trim();
                        changed = true;
                    }

                    if (altByName.BatchUomId != alt.BatchUomId)
                    {
                        altByName.BatchUomId = alt.BatchUomId;
                        changed = true;
                    }

                    if (altByName.MassUomId != alt.MassUomId)
                    {
                        altByName.MassUomId = alt.MassUomId;
                        changed = true;
                    }

                    if (altByName.MassUomValue != alt.MassUomValue)
                    {
                        altByName.MassUomValue = alt.MassUomValue;
                        changed = true;
                    }

                    if (altByName.ResourceBatchUomId != alt.ResourceBatchUomId)
                    {
                        altByName.ResourceBatchUomId = alt.ResourceBatchUomId;
                        changed = true;
                    }

                    if (altByName.ResourceUomId != alt.ResourceUomId)
                    {
                        altByName.ResourceUomId = alt.ResourceUomId;
                        changed = true;
                    }

                    if (altByName.ResourceUomValue != alt.ResourceUomValue)
                    {
                        altByName.ResourceUomValue = alt.ResourceUomValue;
                        changed = true;
                    }

                    if (altByName.PackUomId != alt.PackUomId)
                    {
                        altByName.PackUomId = alt.PackUomId;
                        changed = true;
                    }

                    if (altByName.PackUomValue != alt.PackUomValue)
                    {
                        altByName.PackUomValue = alt.PackUomValue;
                        changed = true;
                    }

                    if (changed)
                    {
                        forBulkUpdateNA.Add(altByName);
                    }
                }
                else
                {
                    var naId = Guid.NewGuid();
                    altByName = new NomenclatureAlternative
                    {
                        Id = naId,
                        Name = altNomName,
                        Code = alt.Code?.Trim(),
                        BatchUomId = alt.BatchUomId,
                        MassUomId = alt.MassUomId,
                        MassUomValue = alt.MassUomValue,
                        ResourceBatchUomId = alt.ResourceBatchUomId,
                        ResourceUomId = alt.ResourceUomId,
                        ResourceUomValue = alt.ResourceUomValue,
                        NomenclatureId = alt.NomenclatureId,
                        PackUomId = alt.PackUomId,
                        PackUomValue = alt.PackUomValue,
                        OwnerId = ownerId
                    };

                    var link = new NomenclatureAlternativeLink
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = clientType == ClientType.Customer ? clientId : (Guid?)null,
                        SupplierId = clientType == ClientType.Supplier ? clientId : (Guid?)null,
                        AlternativeId = naId
                    };

                    forBulkInsertNA.Add(altByName);
                    forBulkInsertLink.Add(link);
                }
            }

            if (forBulkInsertNA.Any())
            {
                _db.BulkInsert(forBulkInsertNA);
                _db.BulkInsert(forBulkInsertLink);
            }

            if (forBulkUpdateNA.Any())
                _db.BulkUpdate(forBulkUpdateNA);
        }
    }
}
