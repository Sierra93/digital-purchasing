using DigitalPurchasing.ExcelReader;
using System;
using Xunit;

namespace DigitalPurchasing.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var c1 = new ExcelRequestReader();
            c1.ToTable(@"c:\req.xlsx");
            Assert.True(c1 != null);
        }
    }
}
