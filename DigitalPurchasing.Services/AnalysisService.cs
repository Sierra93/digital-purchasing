using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Analysis2;
using DigitalPurchasing.Analysis2.Enums;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class AnalysisService : IAnalysisService
    {
        private ApplicationDbContext _db;
        private readonly ICompetitionListService _competitionListService;

        public AnalysisService(ApplicationDbContext db, ICompetitionListService competitionListService)
        {
            _db = db;
            _competitionListService = competitionListService;
        }

        public AnalysisDataVm GetData(Guid clId)
        {
            var cl = _competitionListService.GetById(clId);
            var qrDelivery = _db.QuotationRequests
                .Include(q => q.Delivery)
                .First(q => q.PurchaseRequestId == cl.PurchaseRequest.Id)
                .Delivery;

            var data = new AnalysisDataVm
            {
                CustomerDeliveryDate = qrDelivery.DeliverAt == DateTime.MinValue ? (DateTime?)null : qrDelivery.DeliverAt
            };

            var order = 0;
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
            
            var option1 = new AnalysisOptions();
            option1.SuppliersCountOptions.Type = SupplierCountType.Equal;
            option1.SuppliersCountOptions.Count = 1;

            var option2 = new AnalysisOptions();
            option2.SuppliersCountOptions.Type = SupplierCountType.Equal;
            option2.SuppliersCountOptions.Count = 2;

            var option3 = new AnalysisOptions();
            option3.SuppliersCountOptions.Type = SupplierCountType.Equal;
            option3.SuppliersCountOptions.Count = 3;

            var option4 = new AnalysisOptions();
            option4.SuppliersCountOptions.Type = SupplierCountType.Equal;
            option4.SuppliersCountOptions.Count = 10;

            var options = new List<AnalysisOptions>
            {
                option1, option2, option3, option4
            };

            foreach (var option in options)
            {
                var coreResult = core.Run(option);
                var totalBySupplier = coreResult.GetTotalBySupplier();
                var variant = new AnalysisDataVm.Variant
                {
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

            return data;
        }
    }
}
