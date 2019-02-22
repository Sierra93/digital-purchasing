using DigitalPurchasing.ExcelReader;
using System;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using Xunit;

namespace DigitalPurchasing.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ExcelRequestReaderTest()
        {
            var c1 = new ExcelRequestReader(new TestColumnNameService());
            var result = c1.ToTable(@"c:\ru1.xlsx", Guid.Empty);
            Assert.True(result != null);
        }

        [Theory]
        [InlineData("azая01","a.z-а - я 0 1")]
        [InlineData(null, null)]
        [InlineData("abc","A BC")]
        public void CleanUpTest(string expected, string actual) => Assert.Equal(expected, actual.CustomNormalize());
    }

    public class TestColumnNameService : IColumnNameService
    {
        public string[] GetNames(TableColumnType type, Guid ownerId)
        {
            switch (type)
            {
                case TableColumnType.Name:
                    return new [] {"Товары (работы, услуги)", "Наименование", "Name"};
                case TableColumnType.Qty:
                    return new [] {"Количество"};
                case TableColumnType.Price:
                    return new [] {"Цена"};
                default:
                    return new string[0];
            }
        }

        public void SaveName(TableColumnType type, string name) => throw new NotImplementedException();

        public void SaveAllNames(ColumnResponse model) => throw new NotImplementedException();

        public ColumnResponse GetAllNames() => throw new NotImplementedException();
    }
}
