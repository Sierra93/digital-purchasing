using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalPurchasing.Services
{
    public sealed class FileService : IFileService
    {
        private readonly ApplicationDbContext _db;

        public FileService(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public Guid ReplaceTermsFile(string fileName, byte[] bytes, string contentType)
        {
            var file = _db.Files.OfType<TermsFile>().FirstOrDefault();
            if (file == null)
            {
                file = new TermsFile();
                _db.TermsFiles.Add(file);
            }

            file.Bytes = bytes;
            file.ContentType = contentType;
            file.FileName = fileName;
            
            _db.SaveChanges();
            return file.Id;
        }

        public TermsFileDto GetTermsFile() =>
            _db.Files.OfType<TermsFile>().FirstOrDefault()?.Adapt<TermsFileDto>();
    }
}
