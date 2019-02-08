using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core;
using DigitalPurchasing.Analysis2;
using DigitalPurchasing.Analysis2.Enums;
using Xunit;

namespace DigitalPurchasing.Tests
{
    public class AnalysisTests
    {
        private (AnalysisCustomer Customer, List<AnalysisSupplier> Suppliers) GetTestData()
        {
            var itemId1 = new Guid("5e654ae674ce4e03be619ec6c5701b21");
            var itemId2 = new Guid("d81e5c867aec40ffadbb8f71994d8443");
            var itemId3 = new Guid("a2141f27aa194b4ea111febcf31df950");

            var customer = new AnalysisCustomer
            {
                Id = new Guid("01baea5c472d4b5aace9bf7dbaf013c4"),
                Date = DateTime.UtcNow.Date.AddDays(10),
                Items =
                {
                    new CustomerItem {Id = itemId1, Quantity = 100},
                    new CustomerItem {Id = itemId2, Quantity = 100},
                    new CustomerItem {Id = itemId3, Quantity = 100}
                }
            };

            var supplier1 = new AnalysisSupplier
            {
                Id = new Guid("b9a46c13564c410f955961ba56210fd4"),
                Date = DateTime.UtcNow.Date.AddDays(11),
                DeliveryTerms = DeliveryTerms.CustomerWarehouse,
                PaymentTerms = PaymentTerms.Prepay,
                Items =
                {
                    new SupplierItem {Id = itemId1, Quantity = 100, Price = 10},
                    new SupplierItem {Id = itemId2, Quantity = 100, Price = 11},
                    new SupplierItem {Id = itemId3, Quantity = 100, Price = 12}
                }
            };

            var supplier2 = new AnalysisSupplier
            {
                Id = new Guid("e2fe141095f64dc0b062c0f09d136343"),
                Date = DateTime.UtcNow.Date.AddDays(9),
                Items =
                {
                    new SupplierItem {Id = itemId1, Quantity = 100, Price = 15},
                    new SupplierItem {Id = itemId2, Quantity = 100, Price = 10},
                    new SupplierItem {Id = itemId3, Quantity = 100, Price = 10}
                }
            };

            var supplier3 = new AnalysisSupplier
            {
                Id = new Guid("febef0cdf07d4c5b81955b541aa31953"),
                Date = DateTime.UtcNow.Date.AddDays(8),
                Items =
                {
                    new SupplierItem {Id = itemId1, Quantity = 100, Price = 13},
                    new SupplierItem {Id = itemId2, Quantity = 100, Price = 9},
                    new SupplierItem {Id = itemId3, Quantity = 100, Price = 11}
                }
            };

            return ( customer, new List<AnalysisSupplier> {supplier1, supplier2, supplier3} );
        }

        private AnalysisCore CreateAnalysisCoreWTestData()
        {
            var testData = GetTestData();

            return new AnalysisCore
            {
                Customer = testData.Customer,
                Suppliers = testData.Suppliers,
            };
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SupplierCount_Equal(int suppliersCount)
        {
            var testData = GetTestData();

            var core = new AnalysisCore
            {
                Customer = testData.Customer,
                Suppliers = testData.Suppliers,
            };

            var options = new AnalysisOptions
            {
                SuppliersCountOptions =
                {
                    Count = suppliersCount,
                    Type = SupplierCountType.Equal
                }
            };

            var result = core.Run(options);

            Assert.Equal(3, result.Data.Count);
            Assert.Equal(suppliersCount, result.Data.Select(q => q.Supplier).Distinct().Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SupplierCount_LessOrEqual(int suppliersCount)
        {
            var testData = GetTestData();

            var core = new AnalysisCore
            {
                Customer = testData.Customer,
                Suppliers = testData.Suppliers,
            };

            var options = new AnalysisOptions
            {
                SuppliersCountOptions =
                {
                    Count = suppliersCount,
                    Type = SupplierCountType.LessOrEqual
                }
            };

            var result = core.Run(options);

            Assert.Equal(3, result.Data.Count);
            Assert.True(result.Data.Select(q => q.Supplier).Distinct().Count() <= suppliersCount);
        }

        [Theory]
        [InlineData(DeliveryDateTerms.Min)]
        [InlineData(DeliveryDateTerms.LessThanInRequest)]
        public void DeliveryDateType_Variants(DeliveryDateTerms deliveryDate)
        {
            var testData = GetTestData();

            var core = new AnalysisCore
            {
                Customer = testData.Customer,
                Suppliers = testData.Suppliers,
            };

            var options = new AnalysisOptions
            {
                DeliveryDateTermsOptions =
                {
                    DeliveryDateTerms = deliveryDate
                }
            };

            var result = core.Run(options);

            Assert.Equal(3, result.Data.Count);
            if (deliveryDate == DeliveryDateTerms.LessThanInRequest)
            {
                var validSuppliers = testData.Suppliers.Where(q => q.Date <= testData.Customer.Date).Select(q => q.Id).ToList();
                Assert.All(result.Data.Select(q => q.Supplier.Id), q =>
                {
                    Assert.Contains(q, validSuppliers);
                });
            }
            else if (deliveryDate == DeliveryDateTerms.Min)
            {
                var minDate = testData.Suppliers.Min(q => q.Date);
                Assert.True(result.Data.Select(q => q.Supplier).All(q => q.Date == minDate));
            }
        }

        [Fact]
        public void TotalValue_0()
        {
            var core = CreateAnalysisCoreWTestData();

            var options = new AnalysisOptions
            {
                TotalValueOptions =
                {
                    Value = 0
                }
            };

            var result = core.Run(options);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void TotalValue_3000()
        {
            var core = CreateAnalysisCoreWTestData();

            var options = new AnalysisOptions
            {
                TotalValueOptions =
                {
                    Value = 3000
                }
            };

            var result = core.Run(options);

            Assert.True(result.IsSuccess);
            // 2 suppliers with s date
            Assert.True(result.TotalValue <= 3000);
        }

        [Fact]
        public void PaymentTerms_Prepay()
        {
            var analysisCore = CreateAnalysisCoreWTestData();

            var options = new AnalysisOptions
            {
                PaymentTermsOptions =
                {
                    PaymentTerms = PaymentTerms.Prepay
                }
            };

            var result = analysisCore.Run(options);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Data.Select(q => q.Supplier.Id).Distinct());
        }

        [Theory]
        [InlineData(DeliveryTerms.CustomerWarehouse)]
        [InlineData(DeliveryTerms.DAP)]
        public void DeliveryTerms_Custom(DeliveryTerms deliveryTerms)
        {
            var core = CreateAnalysisCoreWTestData();

            var options = new AnalysisOptions
            {
                DeliveryTermsOptions =
                {
                    DeliveryTerms = deliveryTerms
                }
            };

            var result = core.Run(options);

            Assert.Equal(
                GetTestData().Suppliers.Count(q => q.DeliveryTerms == deliveryTerms),
                result.Data.Select(q => q.Supplier.Id).Distinct().Count());
        }

        [Fact]
        public void DeliveryTerms_NoRequirements()
        {
            var core = CreateAnalysisCoreWTestData();

            var options = new AnalysisOptions
            {
                DeliveryTermsOptions =
                {
                    DeliveryTerms = DeliveryTerms.NoRequirements
                }
            };

            var result = core.Run(options);

            Assert.Equal(
                GetTestData().Suppliers.Count,
                result.Data.Select(q => q.Supplier.Id).Distinct().Count());
        }

        [Fact]
        public void LowItemQuantity()
        {
            var itemId1 = new Guid("5e654ae674ce4e03be619ec6c5701b21");
            var itemId2 = new Guid("d81e5c867aec40ffadbb8f71994d8443");
            var itemId3 = new Guid("a2141f27aa194b4ea111febcf31df950");

            var customer = new AnalysisCustomer
            {
                Id = new Guid("01baea5c472d4b5aace9bf7dbaf013c4"),
                Date = DateTime.UtcNow.Date.AddDays(10),
                Items =
                {
                    new CustomerItem {Id = itemId1, Quantity = 100},
                    new CustomerItem {Id = itemId2, Quantity = 100},
                    new CustomerItem {Id = itemId3, Quantity = 100}
                }
            };

            var supplier1 = new AnalysisSupplier
            {
                Id = new Guid("b9a46c13564c410f955961ba56210fd4"),
                Date = DateTime.UtcNow.Date.AddDays(11),
                Items =
                {
                    new SupplierItem {Id = itemId1, Quantity = 1, Price = 10},
                    new SupplierItem {Id = itemId2, Quantity = 1, Price = 11},
                    new SupplierItem {Id = itemId3, Quantity = 1, Price = 12}
                }
            };

            var core = new AnalysisCore
            {
                Customer = customer,
                Suppliers = new List<AnalysisSupplier> { supplier1 }
            };

            var result = core.Run(new AnalysisOptions());

            Assert.Empty(result.Data);
        }

        [Fact]
        public void MixSuppliers()
        {
            var itemId1 = new Guid("5e654ae674ce4e03be619ec6c5701b21");
            var itemId2 = new Guid("b5f59198455c4646863a3589d2ec2f94");

            var customer = new AnalysisCustomer
            {
                Id = new Guid("01baea5c472d4b5aace9bf7dbaf013c4"),
                Date = DateTime.UtcNow.Date.AddDays(10),
                Items =
                {
                    new CustomerItem { Id = itemId1, Quantity = 100},
                    new CustomerItem { Id = itemId2, Quantity = 100}
                }
            };

            var supplier1 = new AnalysisSupplier
            {
                Id = new Guid("b9a46c13564c410f955961ba56210fd4"),
                Date = DateTime.UtcNow.Date.AddDays(11),
                Items =
                {
                    new SupplierItem { Id = itemId1, Quantity = 100, Price = 10}
                }
            };

            var supplier2 = new AnalysisSupplier
            {
                Id = new Guid("01edff1937fc4ef9a0c326e28c41c3d7"),
                Date = DateTime.UtcNow.Date.AddDays(11),
                Items =
                {
                    new SupplierItem { Id = itemId2, Quantity = 100, Price = 11}
                }
            };

            var core = new AnalysisCore
            {
                Customer = customer,
                Suppliers = new List<AnalysisSupplier> { supplier1, supplier2 }
            };

            var result = core.Run(new AnalysisOptions());

            Assert.True(result != null);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.SuppliersCount);
        }

        [Fact]
        public void Supplier_MixQuantity()
        {
            var itemId1 = new Guid("5e654ae674ce4e03be619ec6c5701b21");

            var customerQty = 100m;

            var customer = new AnalysisCustomer
            {
                Id = new Guid("01baea5c472d4b5aace9bf7dbaf013c4"),
                Date = DateTime.UtcNow.Date.AddDays(10),
                Items =
                {
                    new CustomerItem { Id = itemId1, Quantity = customerQty }
                }
            };

            var supplier1 = new AnalysisSupplier
            {
                Id = new Guid("b9a46c13564c410f955961ba56210fd4"),
                Date = DateTime.UtcNow.Date.AddDays(11),
                Items =
                {
                    new SupplierItem { Id = itemId1, Quantity = 50, Price = 10}
                }
            };

            var supplier2 = new AnalysisSupplier
            {
                Id = new Guid("01edff1937fc4ef9a0c326e28c41c3d7"),
                Date = DateTime.UtcNow.Date.AddDays(11),
                Items =
                {
                    new SupplierItem { Id = itemId1, Quantity = 70, Price = 11}
                }
            };

            var core = new AnalysisCore
            {
                Customer = customer,
                Suppliers = new List<AnalysisSupplier> { supplier1, supplier2 }
            };

            var result = core.Run(new AnalysisOptions());

            Assert.True(result != null);
            Assert.True(result.IsSuccess);
            Assert.Equal(core.Suppliers.Count, result.SuppliersCount);
            Assert.Equal(customerQty, result.Data.Sum(q => q.Item.Quantity));
        }

        [Fact]
        public void Quantity()
        {
            var itemId1 = new Guid("5e654ae674ce4e03be619ec6c5701b21");
            var itemId2 = new Guid("96d9151eb8f24d7b80a2f2b69fc2e1bc");
            var itemId3 = new Guid("8916d8409b32431abefa29ed7fd7b785");

            var customer = new AnalysisCustomer
            {
                Id = new Guid("01baea5c472d4b5aace9bf7dbaf013c4"),
                Items =
                {
                    new CustomerItem { Id = itemId1, Quantity = 100 },
                    new CustomerItem { Id = itemId2, Quantity = 100 },
                    new CustomerItem { Id = itemId3, Quantity = 100 }
                }
            };

            var supplier1 = new AnalysisSupplier
            {
                Id = new Guid("f0477905690c44cbbfa185800db1a6ad"),
                Items =
                {
                    new SupplierItem { Id = itemId1, Quantity = 90, Price = 10 },
                    new SupplierItem { Id = itemId2, Quantity = 90, Price = 10 },
                    new SupplierItem { Id = itemId3, Quantity = 90, Price = 10 }
                }
            };
            
            var supplier2 = new AnalysisSupplier
            {
                Id = new Guid("779add5fdc394cca9bfd7672bce61607"),
                Items =
                {
                    new SupplierItem { Id = itemId1, Quantity = 100, Price = 9 },
                    new SupplierItem { Id = itemId2, Quantity = 100, Price = 8 },
                    new SupplierItem { Id = itemId3, Quantity = 100, Price = 7 }
                }
            };

            var core = new AnalysisCore
            {
                Customer = customer,
                Suppliers = new List<AnalysisSupplier> { supplier1, supplier2 }
            };

            var result = core.Run(new AnalysisOptions());

            Assert.True(result.IsSuccess);
            Assert.All(result.Data, q =>
            {
                Assert.True(q.Item.Quantity == 90
                            || q.Item.Quantity == 100
                            || q.Item.Quantity == 10);
            });
        }

    }
}
