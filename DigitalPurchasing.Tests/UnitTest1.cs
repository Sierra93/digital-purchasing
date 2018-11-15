using DigitalPurchasing.ExcelReader;
using System;
using DigitalPurchasing.Core.Interfaces;
using Xunit;

namespace DigitalPurchasing.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var c1 = new ExcelRequestReader(new TestColumnNameService());
            var result = c1.ToTable(@"c:\ru1.xlsx");
            Assert.True(result != null);
        }
    }

    public class TestColumnNameService : IColumnNameService
    {
        public string[] GetNames(TableColumnType type)
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
