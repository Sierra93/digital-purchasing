using System;
using System.Collections.Generic;
using System.Linq;
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


        public static string CleanPhoneNumber(this string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            return new string(str.Where(char.IsDigit).ToArray());
        }

        public static string LastSymbols(this string str, int symbolsNumber) =>
            str?.Substring(Math.Max(0, str.Length - symbolsNumber));

        public static string FormatPhoneNumber(this string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (str.Length == 10)
            {
                return $"+7 ({str.Substring(0,3)}) {str.Substring(3,3)} {str.Substring(6,2)} {str.Substring(8,2)}";
            }
            else if (str.Length == 11)
            {
                return $"+7 ({str.Substring(1, 3)}) {str.Substring(4, 3)} {str.Substring(7, 2)} {str.Substring(9, 2)}";
            }

            return str;
        }

        public static string ToMD5(this string str)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                for (var i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string ReplaceSpacesWithOneSpace(this string str) =>
            string.IsNullOrWhiteSpace(str) ? str : Regex.Replace(str, @"\s+", " ");

        public static string RemoveSpaces(this string str) =>
            string.IsNullOrWhiteSpace(str) ? str : Regex.Replace(str, @"\s+", "");

        public static IEnumerable<string> Ngrams(this string str, int len)
        {
            var parts = str.ReplaceSpacesWithOneSpace().Split(" ");
            foreach (var p in parts.Where(_ => _.Length >= len))
            {
                for (int i = 0; i <= p.Length - len; i++)
                {
                    yield return p.Substring(i, len);
                }
            }
        }
    }
}
