using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DigitalPurchasing.Core.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex CleanUpRegex = new Regex(@"[^\p{L}\d]", RegexOptions.Compiled);

        public static string CustomNormalize(this string input) =>
            string.IsNullOrEmpty(input) ? null : CleanUpRegex.Replace(input, string.Empty).ToLowerInvariant();
    }
}
