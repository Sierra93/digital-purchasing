using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Analysis;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Core.Interfaces.Analysis;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using AnalysisSupplier = DigitalPurchasing.Analysis.AnalysisSupplier;

namespace DigitalPurchasing.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICompetitionListService _competitionListService;
        private readonly INomenclatureService _nomenclatureService;
        private readonly IRootService _rootService;
        private readonly ISupplierOfferService _supplierOfferService;

        public AnalysisService(
            ApplicationDbContext db,
            ICompetitionListService competitionListService,
            INomenclatureService nomenclatureService,
            IRootService rootService, ISupplierOfferService supplierOfferService)
        {
            _db = db;
            _competitionListService = competitionListService;
            _nomenclatureService = nomenclatureService;
            _rootService = rootService;
            _supplierOfferService = supplierOfferService;
        }

        public AnalysisDataVm GetData(Guid clId)
        {
            var cl = _competitionListService.GetById(clId);

            var data = GetAnalysisData(cl);
            var persons = CreateAnalysisPersons(cl);
            var variants = GetVariants(clId);

            var coreVariants = variants.Select(ToCoreVariant).ToArray();

            var results = new AnalysisCore(persons.Customer, persons.Suppliers).Run(coreVariants);

            foreach (var variant in variants)
            {
                AddVariantToData(data, variant, results.Find(q => q.VariantId == variant.Id));
            }

            data.SelectedVariant = variants.FirstOrDefault(q => q.IsSelected)?.Id;
            data.CustomerRequest = new AnalysisDataVm.CustomerRequestData
            {
                Id = cl.PurchaseRequest.Id,
                Name = cl.PurchaseRequest.Customer.Name,
                CustomerId = cl.PurchaseRequest.Customer.Id
            };

            return data;
        }

        public List<AnalysisDataVm.Variant> GetDefaultVariants()
        {
            var ids = GetDefaultVariantsIds();
            var variants = _db.AnalysisVariants.Where(q => ids.Contains(q.Id)).ToList();

            return variants.Select(q => new AnalysisDataVm.Variant
            {
                Id = q.Id,
                CreatedOn = q.CreatedOn
            }).ToList();
        }

        private void AddVariantToData(AnalysisDataVm data, AnalysisVariant variant, AnalysisResult result)
        {
            var totalBySupplier = result.GetTotalBySupplierOffer();
            var coreVariant = ToCoreVariant(variant);
            var dataVariant = new AnalysisDataVm.Variant
            {
                Id = coreVariant.Id,
                CreatedOn = coreVariant.CreatedOn,
                Results = totalBySupplier.Select(q => new AnalysisDataVm.ResultBySupplierOffer
                {
                    SupplierOfferId = q.Key,
                    Total = q.Value,
                    Order = data.SupplierOffers.Find(w => w.Id == q.Key).Order
                }).ToList(),
                ResultsByItem = result.Data.Select(q => new AnalysisDataVm.ResultByItem
                {
                    SupplierOfferId = q.SupplierOfferId,
                    ItemId = q.NomenclatureId,
                    Quantity = q.Quantity,
                    Price = q.Price
                }).ToList()
            };

            foreach (var supplierOffer in data.SupplierOffers)
            {
                if (dataVariant.Results.All(q => q.SupplierOfferId != supplierOffer.Id))
                {
                    dataVariant.Results.Add(new AnalysisDataVm.ResultBySupplierOffer
                    {
                        SupplierOfferId = supplierOffer.Id,
                        Total = 0,
                        Order = data.SupplierOffers.Find(w => w.Id == supplierOffer.Id).Order
                    });
                }
            }

            data.Variants.Add(dataVariant);
        }

        private AnalysisDataVm GetAnalysisData(CompetitionListVm cl)
        {
            var qrDelivery = _db.QuotationRequests
                .Include(q => q.Delivery)
                .First(q => q.PurchaseRequestId == cl.PurchaseRequest.Id)
                .Delivery;

            var data = new AnalysisDataVm
            {
                CustomerDeliveryDate = qrDelivery.DeliverAt == DateTime.MinValue ? (DateTime?)null : qrDelivery.DeliverAt
            };

            var order = 0;
            foreach (var q in cl.SupplierOffers.Where(q => q.Supplier != null))
            {
                var so = _db.SupplierOffers.Find(q.Id);
                data.SupplierOffers.Add(new AnalysisDataVm.SupplierOfferData
                {
                    Id  = q.Id,
                    Name = q.SupplierName,
                    Order = order++,
                    DeliveryTerms = q.DeliveryTerms,
                    PayWithinDays = so.PayWithinDays,
                    PaymentTerms = q.PaymentTerms,
                    DeliveryDate = so.DeliveryDate == DateTime.MinValue ? (DateTime?)null : so.DeliveryDate,
                    SupplierId = q.Supplier.Id
                });
            }

            return data;
        }

        private (AnalysisCustomer Customer, List<AnalysisSupplier> Suppliers) CreateAnalysisPersons(CompetitionListVm cl)
        {
            var customer = new AnalysisCustomer(
                cl.PurchaseRequest.Customer.Id,
                cl.PurchaseRequest.Id,
                null,
                cl.PurchaseRequest.Items
                    .Select(q => new AnalysisCustomerItem(q.NomenclatureId, q.RawQty))
            );

            var suppliers = cl.SupplierOffers.Where(q => q.Supplier != null).Select(q =>
            {
                var soDetails = _supplierOfferService.GetDetailsById(q.Id);

                return new AnalysisSupplier(
                    q.Supplier.Id,
                    q.Id,
                    null,
                    q.Items.Where(w => w != null).Select(w =>
                    {
                        var soDetailsItem = soDetails.Items.Find(d => d.Offer.ItemId == w.Id);
                        return new AnalysisSupplierItem(w.NomenclatureId,
                            soDetailsItem.Conversion.OfferQty,
                            soDetailsItem.ResourceConversion.OfferPrice
                        );
                    }));
            }).ToList();

            return ( Customer: customer, Suppliers: suppliers );
        }

        private List<AnalysisVariant> GetVariants(Guid clId)
        {
            var variants = _db.AnalysisVariants.Where(q => q.CompetitionListId == clId).ToList();
            if (!variants.Any())
            {
                var ids = GetDefaultVariantsIds();

                var defaultVariants = _db.AnalysisVariants.AsNoTracking().Where(q => ids.Contains(q.Id)).ToList();
                foreach (var variant in defaultVariants)
                {
                    variant.Id = Guid.NewGuid();
                    variant.CompetitionListId = clId;
                }
                _db.AnalysisVariants.AddRange(defaultVariants);
                _db.SaveChanges();

                return GetVariants(clId);
            }

            return variants;
        }

        private void CreateDefaultVariants()
        {
            var v1 = new AnalysisVariant { CompetitionListId = null, CreatedOn = DateTime.UtcNow.AddMilliseconds(1) };
            var v2 = new AnalysisVariant { CompetitionListId = null, CreatedOn = DateTime.UtcNow.AddMilliseconds(2) };
            var v3 = new AnalysisVariant { CompetitionListId = null, CreatedOn = DateTime.UtcNow.AddMilliseconds(3) };

            v1.DeliveryTerms = v2.DeliveryTerms = v3.DeliveryTerms = DeliveryTerms.CustomerWarehouse;
            v1.DeliveryDateTerms = v2.DeliveryDateTerms = v3.DeliveryDateTerms = DeliveryDateTerms.LessThanInRequest;
            v1.PaymentTerms = v2.PaymentTerms = v3.PaymentTerms = PaymentTerms.Postponement;
            v1.SupplierCount = 1;
            v2.SupplierCount = 2;
            v1.SupplierCountType = SupplierCountType.Equal;
            v2.SupplierCountType = SupplierCountType.LessOrEqual;

            _db.AnalysisVariants.AddRange(v1, v2, v3);
            _db.SaveChanges();
        }

        public List<Guid> GetDefaultVariantsIds()
        {
            var ids = _db.AnalysisVariants.Where(q => q.CompetitionListId == null).Select(q => q.Id).ToList();
            if (!ids.Any())
            {
                CreateDefaultVariants();
                return GetDefaultVariantsIds();
            }

            return ids;
        }

        public AnalysisVariantOptions GetVariantData(Guid vId)
        {
            var av = _db.AnalysisVariants.Find(vId);
            var result = av.Adapt<AnalysisVariantOptions>();
            return result;
        }

        public AnalysisDataVm.Variant AddDefaultVariant()
        {
            var av = new AnalysisVariant
            {
                CompetitionListId = null
            };

            var entry = _db.AnalysisVariants.Add(av);
            _db.SaveChanges();
            av = entry.Entity;
            return new AnalysisDataVm.Variant {Id = av.Id, CreatedOn = av.CreatedOn};
        }

        public AnalysisDataVm AddVariant(Guid clId)
        {
            var cl = _competitionListService.GetById(clId);

            var data = GetAnalysisData(cl);
            var persons = CreateAnalysisPersons(cl);

            var av = new AnalysisVariant
            {
                CompetitionListId = clId
            };

            var entry = _db.AnalysisVariants.Add(av);
            _db.SaveChanges();

            var variant = entry.Entity;

            AddVariantToData(data, variant, new AnalysisCore(persons.Customer, persons.Suppliers).Run(ToCoreVariant(variant)));

            return data;
        }

        public void SaveVariant(AnalysisSaveVariant variant)
        {
            var entity = _db.AnalysisVariants.Find(variant.Id);
            entity = variant.Adapt(entity);
            _db.SaveChanges();
        }

        public void DeleteVariant(Guid id)
        {
            _db.Remove(_db.AnalysisVariants.Find(id));
            _db.SaveChanges();
        }

        public AnalysisDetails GetDetails(Guid clId)
        {
            var cl = _competitionListService.GetById(clId);
            var persons = CreateAnalysisPersons(cl);
            var variants = GetVariants(clId);
            
            var coreOptions = variants.Select(ToCoreVariant).ToArray();
            var analysisResults = new AnalysisCore(persons.Customer, persons.Suppliers).Run(coreOptions);
            var variantResults = new Dictionary<AnalysisResult, AnalysisVariantData>(analysisResults.Count);

            foreach (var analysisResult in analysisResults)
            {
                variantResults.Add(analysisResult, coreOptions.First(q => q.Id == analysisResult.VariantId));
            }

            var result = new AnalysisDetails();

            foreach (var customerItem in persons.Customer.Items)
            {
                var nomenclature = _nomenclatureService.GetById(customerItem.NomenclatureId);
                var item = new AnalysisDetails.Item
                {
                    Id = customerItem.NomenclatureId,
                    Code = nomenclature.Code,
                    Name = nomenclature.Name,
                    Quantity = customerItem.Quantity,
                    Uom = nomenclature.BatchUomName,
                    // todo: fill with real data
                    Currency = "RUB"
                };

                var variantIndex = 0;
                foreach (var variantResult in variantResults.Where(q => q.Key.IsSuccess).OrderBy(q => q.Value.CreatedOn))
                {
                    var supplierOfferIds = variantResult.Key.Data.Select(q => q.SupplierOfferId).Distinct().ToList();

                    var variant = new AnalysisDetails.Variant
                    {
                        Name = $"Вариант {++variantIndex}",
                        CreatedOn = variantResult.Value.CreatedOn
                    };

                    foreach (var variantItemData in variantResult.Key.Data.Where(q => q.NomenclatureId == item.Id))
                    {
                        if (variant.Suppliers.Any(q => q.Key.Id == variantItemData.SupplierOfferId))
                        {
                            var supplier = variant.Suppliers
                                .First(q => q.Key.Id == variantItemData.SupplierOfferId);
                            supplier.Value.TotalPrice += variantItemData.TotalPrice;
                        }
                        else
                        {
                            var so = _db.SupplierOffers.Find(variantItemData.SupplierOfferId);
                            var supplier = new AnalysisDetails.Supplier
                            {
                                Id = so.Id,
                                Name = so.SupplierName,
                            };
                            var supplierData = new AnalysisDetails.SupplierData
                            {
                                TotalPrice = variantItemData.TotalPrice
                            };
                            variant.Suppliers.Add(supplier, supplierData);
                        }
                    }

                    foreach (var supplierOfferId in supplierOfferIds)
                    {
                        if (variant.Suppliers.All(q => q.Key.Id != supplierOfferId))
                        {
                            var so = _db.SupplierOffers.Find(supplierOfferId);
                            var supplier = new AnalysisDetails.Supplier
                            {
                                Id = so.Id,
                                Name = so.SupplierName,
                            };
                            variant.Suppliers.Add(supplier, new AnalysisDetails.SupplierData());
                        }
                    }

                    item.Variants.Add(variant);
                }
                result.Items.Add(item);
            }

            return result;
        }

        public async Task SelectVariant(Guid variantId)
        {
            var variant = await _db.AnalysisVariants.FindAsync(variantId);
            if (variant.CompetitionListId.HasValue)
            {
                var clId = variant.CompetitionListId.Value;
                var allVariants = await _db.AnalysisVariants
                    .Where(q => q.CompetitionListId == clId)
                    .ToListAsync();
                allVariants.ForEach(q => q.IsSelected = false);
                variant.IsSelected = true;
                _db.SaveChanges();

                var rootId = await _rootService.GetIdByCL(clId);
                await _rootService.SetStatus(rootId, RootStatus.SupplierSelected);
            } 
        }

        private AnalysisVariantData ToCoreVariant(AnalysisVariant av)
        {
            var op = new AnalysisVariantData
            {
                Id = av.Id,
                CreatedOn = av.CreatedOn,
                DeliveryTermsOptions = { DeliveryTerms = av.DeliveryTerms },
                DeliveryDateTermsOptions = { DeliveryDateTerms = av.DeliveryDateTerms },
                PaymentTermsOptions = { PaymentTerms = av.PaymentTerms },
                SuppliersCountOptions = { Count = av.SupplierCount, Type = av.SupplierCountType },
                TotalValueOptions = { Value = av.TotalValue }
            };

            return op;
        }
    }
}
