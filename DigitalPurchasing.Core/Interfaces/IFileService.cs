using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IFileService
    {
        Guid ReplaceTermsFile(string fileName, byte[] bytes, string contentType);
        TermsFileDto GetTermsFile();
    }

    public class TermsFileDto
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
