using DigitalPurchasing.ExcelReader;
using System;
using System.Linq;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using Xunit;

namespace DigitalPurchasing.Tests
{
    public class ExcelTests
    {
        [Fact]
        public void ExcelRequestReaderTest()
        {
            var c1 = new ExcelRequestReader(new TestColumnNameService());
            var result = c1.ToTable(@"c:\ru1.xlsx", Guid.Empty);
            Assert.True(result != null);
        }

        [Fact]
        public void CanReadBinaryFormat()
        {
            var c1 = new ExcelRequestReader(new TestColumnNameService());
            var result = c1.ToTable(@"W:\DP\T1\req1.xls", Guid.Empty);
            Assert.True(result != null);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void CanReadBinaryFormat_BigFiles()
        {
            var c1 = new ExcelRequestReader(new TestColumnNameService());
            var result = c1.ToTable(@"W:\DP\big.xls", Guid.Empty);
            Assert.True(result != null);
            Assert.True(result.IsSuccess);
            Assert.True(result.Table.Columns
                            .First(q => q.Type == TableColumnType.Name)
                            .Values.Count >= 199);
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
                    return new []
                    {
                        "Товары (работы, услуги)",
                        "Наименование", "Name", "Товар",
                        "Наименование работ"
                    };
                case TableColumnType.Qty:
                    return new [] {"Количество"};
                case TableColumnType.Price:
                    return new [] {"Цена"};
                default:
                    return new string[0];
            }
        }

        public void SaveAllNames(ColumnResponse model) => throw new NotImplementedException();

        public ColumnResponse GetAllNames() => throw new NotImplementedException();

        public void SaveName(TableColumnType type, string name, Guid ownerId) => throw new NotImplementedException();
    }
}
