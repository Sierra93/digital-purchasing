using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Analysis2;
using DigitalPurchasing.Analysis2.Enums;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;

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
            var data = new AnalysisDataVm();

            var cl = _competitionListService.GetById(clId);

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
                    data.Suppliers.Add(new AnalysisDataVm.Supplier
                    {
                        Id  = q.Id,
                        Name = q.SupplierName,
                        Order = order++
                    });
                    return new Supplier
                    {
                        Id = q.Id,
                        Items = q.Items.Select(w => new SupplierItem
                        {
                            Id = w.NomenclatureId,
                            Price = w.RawPrice,
                            Quantity = w.RawQty
                        }).ToList()
                    };
                }).ToList()
            };

           

            var option1 = new AnalysisOptions();
            option1.SetSupplierCount(SupplierCountType.Equal, 1);

            var option2 = new AnalysisOptions();
            option2.SetSupplierCount(SupplierCountType.Equal, 2);

            var option3 = new AnalysisOptions();
            option3.SetSupplierCount(SupplierCountType.Equal, 3);

            var option4 = new AnalysisOptions();
            option4.SetSupplierCount(SupplierCountType.Equal, 10);

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
