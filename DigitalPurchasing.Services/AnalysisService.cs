using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Analysis2;
using DigitalPurchasing.Analysis2.Enums;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICompetitionListService _competitionListService;

        public AnalysisService(ApplicationDbContext db, ICompetitionListService competitionListService)
        {
            _db = db;
            _competitionListService = competitionListService;
        }

        public AnalysisDataVm GetData(Guid clId)
        {
            var cl = _competitionListService.GetById(clId);

            var data = GetAnalysisData(cl);
            var core = CreateAnalysisCore(cl);
            var options = GetAnalysisOptions(clId);

            foreach (var option in options)
            {
                AddVariantToData(data, option, core.Run(option));
            }

            return data;
        }

        private void AddVariantToData(AnalysisDataVm data, AnalysisOptions option, AnalysisResult result)
        {
            var totalBySupplier = result.GetTotalBySupplier();
            var variant = new AnalysisDataVm.Variant
            {
                Id = option.Id,
                CreatedOn = option.CreatedOn,
                Results = totalBySupplier.Select(q => new AnalysisDataVm.Result
                {
                    SupplierId = q.Key,
                    Total = q.Value,
                    Order = data.Suppliers.Find(w => w.Id == q.Key).Order
                }).ToList()
            };

            foreach (var supplier in data.Suppliers)
            {
                if (variant.Results.All(q => q.SupplierId != supplier.Id))
                {
                    variant.Results.Add(new AnalysisDataVm.Result
                    {
                        SupplierId = supplier.Id,
                        Total = 0,
                        Order = data.Suppliers.Find(w => w.Id == supplier.Id).Order
                    });
                }
            }

            data.Variants.Add(variant);
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
            foreach (var q in cl.SupplierOffers)
            {
                var so = _db.SupplierOffers.Find(q.Id);
                data.Suppliers.Add(new AnalysisDataVm.Supplier
                {
                    Id  = q.Id,
                    Name = q.SupplierName,
                    Order = order++,
                    DeliveryTerms = q.DeliveryTerms,
                    PayWithinDays = so.PayWithinDays,
                    PaymentTerms = q.PaymentTerms,
                    DeliveryDate = so.DeliveryDate == DateTime.MinValue ? (DateTime?)null : so.DeliveryDate
                });
            }

            return data;
        }

        private AnalysisCore CreateAnalysisCore(CompetitionListVm cl)
        {
            var core = new AnalysisCore
            {
                Customer = new Customer
                {
                    Id = cl.PurchaseRequest.Id,
                    Items = cl.PurchaseRequest.Items.Select(q => new CustomerItem
                    {
                        Id = q.NomenclatureId,
                        Quantity = q.RawQty
                    }).ToList()
                },
                Suppliers = cl.SupplierOffers.Select(q =>
                {
                    return new Supplier
                    {
                        Id = q.Id,
                        DeliveryTerms = q.DeliveryTerms,
                        PaymentTerms = q.PaymentTerms,
                        Items = q.Items.Where(w => w != null).Select(w => new SupplierItem
                        {
                            Id = w.NomenclatureId,
                            Price = w.RawPrice,
                            Quantity = w.RawQty
                        }).ToList()
                    };
                }).ToList()
            };

            return core;
        }

        private List<AnalysisOptions> GetAnalysisOptions(Guid clId)
        {
            var variants = _db.AnalysisVariants.Where(q => q.CompetitionListId == clId).ToList();
            var result = new List<AnalysisOptions>();

            foreach (var variant in variants)
            {
                result.Add(ToOption(variant));
            }

            return result;
        }

        public AnalysisVariantOptions GetVariantData(Guid vId)
        {
            var av = _db.AnalysisVariants.Find(vId);
            var result = av.Adapt<AnalysisVariantOptions>();
            return result;
        }

        public AnalysisDataVm AddVariant(Guid clId)
        {
            var cl = _competitionListService.GetById(clId);

            var data = GetAnalysisData(cl);
            var core = CreateAnalysisCore(cl);

            var av = new AnalysisVariant
            {
                CompetitionListId = clId
            };

            var entry = _db.AnalysisVariants.Add(av);
            _db.SaveChanges();

            var option = ToOption(entry.Entity);

            AddVariantToData(data, option, core.Run(option));

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

        private AnalysisOptions ToOption(AnalysisVariant av)
        {
            var op = new AnalysisOptions
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
