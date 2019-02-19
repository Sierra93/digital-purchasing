using System;
using System.Collections.Generic;
using System.Text;
using DigitalPurchasing.Core.Extensions;
using Xunit;

namespace DigitalPurchasing.Tests
{
    public class Extensions
    {
        [Fact]
        public void TestMD5()
        {
            var md5 = "12:34:56".ToMD5();
            Assert.Equal("95456bfe9e360f986a04f249505f8756", md5);
        }
    }
}
