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
using DigitalPurchasing.Core.Extensions;

namespace DigitalPurchasing.Services
{
    public sealed class NomenclatureAlternativeService : INomenclatureAlternativeService
    {
        private readonly ApplicationDbContext _db;
        private readonly INomenclatureComparisonService _nomenclatureComparisonService;

        public NomenclatureAlternativeService(
            ApplicationDbContext dbContext,
            INomenclatureComparisonService nomenclatureComparisonService)
        {
            _db = dbContext;
            _nomenclatureComparisonService = nomenclatureComparisonService;
        }

        public void UpdateAlternative(NomenclatureAlternativeVm model)
        {
            var entity = _db.NomenclatureAlternatives.Find(model.Id);

            bool isNameChanged = entity.Name != model.Name;

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

            if (isNameChanged)
            {
                _db.NomenclatureComparisonDatas.Remove(
                    _db.NomenclatureComparisonDatas.First(_ => _.NomenclatureId == entity.NomenclatureId && _.NomenclatureAlternativeId == entity.Id));
                _db.SaveChanges();
                var cd = GetComparisonDataByNomenclatureAlt(entity);
                _db.NomenclatureComparisonDatas.Add(cd);
                cd.AdjustedNameNgrams.AddRange(GetNgramsForNomComparisonData(cd, entity.OwnerId));
                _db.SaveChanges();
            }
        }

        public NomenclatureAlternativeVm GetAlternativeById(Guid id)
        {
            var entity = _db.NomenclatureAlternatives
                .Include(q => q.BatchUom)
                .Include(q => q.MassUom)
                .Include(q => q.PackUom)
                .Include(q => q.ResourceUom)
                .Include(q => q.ResourceBatchUom)
                .First(q => q.Id == id);
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

        public Guid? AddNomenclatureForCustomer(Guid prItemId)
        {
            var prItem = _db.PurchaseRequestItems.Include(q => q.PurchaseRequest).First(q => q.Id == prItemId);
            if (prItem.NomenclatureId.HasValue && prItem.PurchaseRequest.CustomerId.HasValue)
            {
                return AddOrUpdateNomenclatureAlts(
                    prItem.PurchaseRequest.OwnerId,
                    prItem.PurchaseRequest.CustomerId.Value,
                    ClientType.Customer,
                    prItem.NomenclatureId.Value,
                    prItem.RawName,
                    prItem.RawCode,
                    prItem.RawUomMatchId);
            }

            return null;
        }

        public Guid? AddNomenclatureForSupplier(Guid soItemId)
        {
            var soItem = _db.SupplierOfferItems.IgnoreQueryFilters().Include(q => q.SupplierOffer).First(q => q.Id == soItemId);
            if (soItem.NomenclatureId.HasValue && soItem.SupplierOffer.SupplierId.HasValue)
            {
                return AddOrUpdateNomenclatureAlts(
                    soItem.SupplierOffer.OwnerId,
                    soItem.SupplierOffer.SupplierId.Value,
                    ClientType.Supplier,
                    soItem.NomenclatureId.Value,
                    soItem.RawName,
                    soItem.RawCode,
                    soItem.RawUomId);
            }
            return null;
        }

        public Guid AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            Guid nomenclatureId, string name, string code, Guid? batchUomId)
        {
            var result = AddOrUpdateNomenclatureAlts(ownerId, clientId, clientType,
                new List<AddOrUpdateAltDto>
                {
                    new AddOrUpdateAltDto
                    {
                        NomenclatureId = nomenclatureId,
                        Name = name,
                        Code = code,
                        BatchUomId = batchUomId
                    }
                });

            return result[0];
        }
        
        public List<Guid> AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            List<AddOrUpdateAltDto> alts)
        {
            if (!alts.Any())
            {
                return new List<Guid>();
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
            var notChangedAlts = new List<Guid>();

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
                    else
                    {
                        notChangedAlts.Add(altByName.Id);
                    }
                }
                else
                {
                    forBulkInsertNA.Add(new NomenclatureAlternative
                    {
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
                    });
                }
            }

            if (forBulkInsertNA.Any())
            {
                _db.BulkInsert(forBulkInsertNA);
                _db.BulkInsert(forBulkInsertNA.Select(alt => new NomenclatureAlternativeLink
                {
                    CustomerId = clientType == ClientType.Customer ? clientId : (Guid?)null,
                    SupplierId = clientType == ClientType.Supplier ? clientId : (Guid?)null,
                    AlternativeId = alt.Id
                }).ToList());
                var compDataItems = forBulkInsertNA.Select(alt => GetComparisonDataByNomenclatureAlt(alt)).ToList();
                _db.BulkInsert(compDataItems);
                _db.BulkInsert(compDataItems.Select(cd => GetNgramsForNomComparisonData(cd, ownerId)).SelectMany(ng => ng).ToList());
            }

            if (forBulkUpdateNA.Any())
            {
                _db.BulkUpdate(forBulkUpdateNA);
            }

            return notChangedAlts
                .Union(forBulkInsertNA.Select(q => q.Id)
                    .Union(forBulkUpdateNA.Select(q => q.Id))) 
                .ToList();
        }

        private IEnumerable<NomenclatureComparisonDataNGram> GetNgramsForNomComparisonData(NomenclatureComparisonData cd, Guid ownerId)
        {
            byte ngramLen = 3;
            return from ng in cd.AdjustedNomenclatureName.Ngrams(ngramLen)
                   select new NomenclatureComparisonDataNGram()
                   {
                       NomenclatureComparisonDataId = cd.Id,
                       N = ngramLen,
                       Gram = ng,
                       OwnerId = ownerId
                   };
        }

        private NomenclatureComparisonData GetComparisonDataByNomenclatureAlt(NomenclatureAlternative alt)
        {
            var terms = _nomenclatureComparisonService.CalculateComparisonTerms(alt.Name);

            return new NomenclatureComparisonData
            {
                AdjustedNomenclatureDigits = terms.AdjustedDigits,
                AdjustedNomenclatureName = terms.AdjustedName,
                NomenclatureDimensions = terms.NomDimensions,
                NomenclatureId = alt.NomenclatureId,
                NomenclatureAlternativeId = alt.Id
            };
        }

        public void Delete(Guid id)
        {
            var entity = _db.NomenclatureAlternatives.Find(id);
            if (entity != null)
            {
                _db.NomenclatureComparisonDatas.RemoveRange(_db.NomenclatureComparisonDatas.Where(cd => cd.NomenclatureAlternativeId == id));
                _db.NomenclatureAlternatives.Remove(entity);
                _db.SaveChanges();
            }
        }

        private NomenclatureAlternativeVm GetByClient(Guid clientId, bool isCustomer)
        {
            var qry = _db.NomenclatureAlternatives.Include(q => q.Link).AsQueryable();
            var entity = isCustomer
                ? qry.FirstOrDefault(q => q.Link.CustomerId == clientId)
                : qry.FirstOrDefault(q => q.Link.SupplierId == clientId);
            return entity.Adapt<NomenclatureAlternativeVm>();
        }

        public NomenclatureAlternativeVm GetForCustomer(Guid customerId) => GetByClient(customerId, true);

        public NomenclatureAlternativeVm GetForSupplier(Guid supplierId) => GetByClient(supplierId, false);
    }
}
