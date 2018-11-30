using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Core;
using DigitalPurchasing.Analysis2;
using DigitalPurchasing.Analysis2.Enums;
using Xunit;

namespace DigitalPurchasing.Tests
{
    public class AnalysisTests
    {
        private (Customer Customer, List<Supplier> Suppliers) GetTestData()
        {
            var itemId1 = new Guid("5e654ae674ce4e03be619ec6c5701b21");
            var itemId2 = new Guid("d81e5c867aec40ffadbb8f71994d8443");
            var itemId3 = new Guid("a2141f27aa194b4ea111febcf31df950");

            var customer = new Customer
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

            var supplier1 = new Supplier
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

            var supplier2 = new Supplier
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

            var supplier3 = new Supplier
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

            return ( customer, new List<Supplier> {supplier1, supplier2, supplier3} );
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

            var options = new AnalysisOptions();
            options.SetSupplierCount(SupplierCountType.Equal, suppliersCount);

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

            var options = new AnalysisOptions();
            options.SetSupplierCount(SupplierCountType.LessOrEqual, suppliersCount);

            var result = core.Run(options);

            Assert.Equal(3, result.Data.Count);
            Assert.True(result.Data.Select(q => q.Supplier).Distinct().Count() <= suppliersCount);
        }

        [Theory]
        [InlineData(DeliveryDateType.Min)]
        [InlineData(DeliveryDateType.LessThanInRequest)]
        public void DeliveryDateType_Variants(DeliveryDateType deliveryDate)
        {
            var testData = GetTestData();

            var core = new AnalysisCore
            {
                Customer = testData.Customer,
                Suppliers = testData.Suppliers,
            };

            var options = new AnalysisOptions();
            options.SetDeliveryDate(deliveryDate);

            var result = core.Run(options);

            Assert.Equal(3, result.Data.Count);
            if (deliveryDate == DeliveryDateType.LessThanInRequest)
            {
                var validSuppliers = testData.Suppliers.Where(q => q.Date <= testData.Customer.Date).Select(q => q.Id).ToList();
                Assert.All(result.Data.Select(q => q.Supplier.Id), q =>
                {
                    Assert.Contains(q, validSuppliers);
                });
            }
            else if (deliveryDate == DeliveryDateType.Min)
            {
                var minDate = testData.Suppliers.Min(q => q.Date);
                Assert.True(result.Data.Select(q => q.Supplier).All(q => q.Date == minDate));
            }
        }

        [Fact]
        public void TotalValue_0()
        {
            var core = CreateAnalysisCoreWTestData();

            var options = new AnalysisOptions();
            options.SetTotalValue(0);

            var result = core.Run(options);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void TotalValue_3000()
        {
            var core = CreateAnalysisCoreWTestData();

            var options = new AnalysisOptions();
            options.SetTotalValue(3000);

            var result = core.Run(options);

            Assert.True(result.IsSuccess);
            // 2 suppliers with s date
            Assert.True(result.TotalValue <= 3000);
        }

        [Fact]
        public void PaymentTerms_Prepay()
        {
            var analysisCore = CreateAnalysisCoreWTestData();

            var options = new AnalysisOptions();
            options.SetPaymentTerms(PaymentTerms.Prepay);

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

            var options = new AnalysisOptions();
            options.SetDeliveryTerms(deliveryTerms);

            var result = core.Run(options);

            Assert.Equal(
                GetTestData().Suppliers.Count(q => q.DeliveryTerms == deliveryTerms),
                result.Data.Select(q => q.Supplier.Id).Distinct().Count());
        }

        [Fact]
        public void DeliveryTerms_NoRequirements()
        {
            var core = CreateAnalysisCoreWTestData();

            var options = new AnalysisOptions();
            options.SetDeliveryTerms(DeliveryTerms.NoRequirements);

            var result = core.Run(options);

            Assert.Equal(
                GetTestData().Suppliers.Count,
                result.Data.Select(q => q.Supplier.Id).Distinct().Count());
        }
    }
}
