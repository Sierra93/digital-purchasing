using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core;
using DigitalPurchasing.Analysis2;
using DigitalPurchasing.Analysis2.Enums;
using DigitalPurchasing.Analysis2.Filters;
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

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SupplierCount_Equal(int suppliersCount)
        {
            var testData = GetTestData();

            var options = new AnalysisCoreVariant
            {
                SuppliersCountOptions =
                {
                    Count = suppliersCount,
                    Type = SupplierCountType.Equal
                }
            };

            var result = new AnalysisCore(testData.Customer, testData.Suppliers).Run(options);

            Assert.Equal(3, result.Data.Count);
            Assert.Equal(suppliersCount, result.Data.Select(q => q.SupplierId).Distinct().Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SupplierCount_LessOrEqual(int suppliersCount)
        {
            var options = new AnalysisCoreVariant
            {
                SuppliersCountOptions =
                {
                    Count = suppliersCount,
                    Type = SupplierCountType.LessOrEqual
                }
            };

            var testData = GetTestData();
            var result = new AnalysisCore(testData.Customer, testData.Suppliers).Run(options);

            Assert.Equal(3, result.Data.Count);
            Assert.True(result.Data.Select(q => q.SupplierId).Distinct().Count() <= suppliersCount);
        }

        [Theory]
        [InlineData(DeliveryDateTerms.Min)]
        [InlineData(DeliveryDateTerms.LessThanInRequest)]
        public void DeliveryDateType_Variants(DeliveryDateTerms deliveryDate)
        {
            var options = new AnalysisCoreVariant
            {
                DeliveryDateTermsOptions =
                {
                    DeliveryDateTerms = deliveryDate
                }
            };

            var testData = GetTestData();
            var result = new AnalysisCore(testData.Customer, testData.Suppliers).Run(options);

            Assert.Equal(3, result.Data.Count);
            if (deliveryDate == DeliveryDateTerms.LessThanInRequest)
            {
                var validSuppliers = testData.Suppliers.Where(q => q.Date <= testData.Customer.Date).Select(q => q.Id).ToList();
                Assert.All(result.Data.Select(q => q.SupplierId), q =>
                {
                    Assert.Contains(q, validSuppliers);
                });
            }
            else if (deliveryDate == DeliveryDateTerms.Min)
            {
                var minDate = testData.Suppliers.Min(q => q.Date);
                Assert.True(result.Data
                    .Select(q => q.SupplierId)
                    .All(q => testData.Suppliers.Find(w => w.Id == q).Date == minDate));
            }
        }

        [Fact]
        public void TotalValue_0()
        {
            var options = new AnalysisCoreVariant
            {
                TotalValueOptions =
                {
                    Value = 0
                }
            };

            var testData = GetTestData();
            var result = new AnalysisCore(testData.Customer, testData.Suppliers).Run(options);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void TotalValue_3000()
        {
            var options = new AnalysisCoreVariant
            {
                TotalValueOptions =
                {
                    Value = 3000
                }
            };

            var testData = GetTestData();
            var result = new AnalysisCore(testData.Customer, testData.Suppliers).Run(options);

            Assert.True(result.IsSuccess);
            // 2 suppliers with s date
            Assert.True(result.TotalValue <= 3000);
        }

        [Fact]
        public void PaymentTerms_Prepay()
        {
            var options = new AnalysisCoreVariant
            {
                PaymentTermsOptions =
                {
                    PaymentTerms = PaymentTerms.Prepay
                }
            };

            var testData = GetTestData();
            var result = new AnalysisCore(testData.Customer, testData.Suppliers).Run(options);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Data.Select(q => q.SupplierId).Distinct());
        }

        [Theory]
        [InlineData(DeliveryTerms.CustomerWarehouse)]
        [InlineData(DeliveryTerms.DAP)]
        public void DeliveryTerms_Custom(DeliveryTerms deliveryTerms)
        {
            var options = new AnalysisCoreVariant
            {
                DeliveryTermsOptions =
                {
                    DeliveryTerms = deliveryTerms
                }
            };

            var testData = GetTestData();
            var result = new AnalysisCore(testData.Customer, testData.Suppliers).Run(options);

            Assert.Equal(
                GetTestData().Suppliers.Count(q => q.DeliveryTerms == deliveryTerms),
                result.Data.Select(q => q.SupplierId).Distinct().Count());
        }

        [Fact]
        public void DeliveryTerms_NoRequirements()
        {
            var options = new AnalysisCoreVariant
            {
                DeliveryTermsOptions =
                {
                    DeliveryTerms = DeliveryTerms.NoRequirements
                }
            };

            var testData = GetTestData();

            var result = new AnalysisCore(testData.Customer, testData.Suppliers).Run(options);

            Assert.Equal(
                GetTestData().Suppliers.Count,
                result.Data.Select(q => q.SupplierId).Distinct().Count());
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

            var result = new AnalysisCore(customer, new List<AnalysisSupplier> { supplier1 }).Run(new AnalysisCoreVariant());

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

            var suppliers = new List<AnalysisSupplier> {supplier1, supplier2};

            var result = new AnalysisCore(customer, suppliers).Run(new AnalysisCoreVariant());

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


            var suppliers = new List<AnalysisSupplier> {supplier1, supplier2};

            var result = new AnalysisCore(customer, suppliers).Run(new AnalysisCoreVariant());

            Assert.True(result != null);
            Assert.True(result.IsSuccess);
            Assert.Equal(suppliers.Count, result.SuppliersCount);
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

            var result = new AnalysisCore(customer, new List<AnalysisSupplier> { supplier1, supplier2 }).Run(new AnalysisCoreVariant());

            Assert.True(result.IsSuccess);
            Assert.All(result.Data, q =>
            {
                Assert.True(q.Item.Quantity == 90
                            || q.Item.Quantity == 100
                            || q.Item.Quantity == 10);
            });
        }

        [Fact]
        public void MultipleOptionsAtOnce()
        {
            var itemId1 = new Guid("5e654ae674ce4e03be619ec6c5701b21");
            var itemId2 = new Guid("96d9151eb8f24d7b80a2f2b69fc2e1bc");

            var customer = new AnalysisCustomer
            {
                Id = new Guid("01baea5c472d4b5aace9bf7dbaf013c4"),
                Items =
                {
                    new CustomerItem { Id = itemId1, Quantity = 100 },
                    new CustomerItem { Id = itemId2, Quantity = 100 },
                }
            };

            var supplier1 = new AnalysisSupplier
            {
                Id = new Guid("f0477905690c44cbbfa185800db1a6ad"),
                Items =
                {
                    new SupplierItem { Id = itemId1, Quantity = 100, Price = 8 },
                    new SupplierItem { Id = itemId2, Quantity = 100, Price = 11 },
                }
            };
            
            var supplier2 = new AnalysisSupplier
            {
                Id = new Guid("779add5fdc394cca9bfd7672bce61607"),
                Items =
                {
                    new SupplierItem { Id = itemId1, Quantity = 100, Price = 10 },
                    new SupplierItem { Id = itemId2, Quantity = 100, Price = 8 },
                }
            };

            var suppliers = new List<AnalysisSupplier> {supplier1, supplier2};

            var core = new AnalysisCore(customer, suppliers);

            var variant1 = new AnalysisCoreVariant
            {
                Id = new Guid("4650979df43843c3874737a2692d8b4d"),
                SuppliersCountOptions = new VariantsSuppliersCountOptions()
                {
                    Count = 1, Type = SupplierCountType.Equal
                }
            };

            var variant2 = new AnalysisCoreVariant
            {
                Id = new Guid("04a895f1f846495ca363693eba8d4b19"),
                SuppliersCountOptions = new VariantsSuppliersCountOptions()
                {
                    Count = 2, Type = SupplierCountType.Equal
                }
            };

            var results = core.Run(variant1, variant2);

            Assert.Equal(2, results.Count);
            Assert.Contains(results, q => q.VariantId == variant1.Id);
            Assert.Contains(results, q => q.VariantId == variant2.Id);
            Assert.Equal(1, results.Count(q => q.SuppliersCount == 1));
            Assert.Equal(1, results.Count(q => q.SuppliersCount == 2));
        }

    }
}
