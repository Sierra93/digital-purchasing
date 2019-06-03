using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Core;
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

        [Fact]
        public void Combintations()
        {
            var list = new[] { 1, 2 };

            var combinations = list.Combinations(); 

            Assert.Equal(4, combinations.Count()); // [{},{1},{2},{1,2}]
            Assert.Single(combinations, q => !q.Any()); // {}
            Assert.Equal(2, combinations.Count(q => q.Count() == 1)); // {1},{2}
            Assert.Single(combinations, q => q.Count() == 2); // {1,2}
        }

        [Fact]
        public void Combintations_Single()
        {
            var list = new[] { 1 };

            var combinations = list.Combinations();

            Assert.Equal(2, combinations.Count()); // [{},{1}]
            Assert.Single(combinations, q=>q.Count() == 1); // [{1}]
            Assert.Single(combinations, q=>q.Count() == 0); // [{}]
        }
    }
}
