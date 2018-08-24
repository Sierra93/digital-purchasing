using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using System;
using System.Collections.Generic;
using System.Text;
using DigitalPurchasing.Models;
using Mapster;
using Newtonsoft.Json;

namespace DigitalPurchasing.Services
{
    public class PurchasingRequestService : IPurchasingRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly IExcelRequestReader _excelRequestReader;

        public PurchasingRequestService(ApplicationDbContext db, IExcelRequestReader excelFileReader)
        {
            _db = db;
            _excelRequestReader = excelFileReader;
        }

        public Guid CreateFromFile(string filePath)
        {
            var table = _excelRequestReader.ToTable(filePath);
            var entry = _db.PurchasingRequests.Add(new PurchasingRequest
            {
                RawData = JsonConvert.SerializeObject(table),
                Type = PurchasingRequestType.AppropriateColumns
            });

            _db.SaveChanges();

            return entry.Entity.Id;
        }

        public PurchasingRequestDetailsResponse GetById(Guid id)
        {
            var entity = _db.PurchasingRequests.Find(id);

            var result = entity.Adapt<PurchasingRequestDetailsResponse>();
            result.ExcelTable =
                entity.RawData != null ? JsonConvert.DeserializeObject<ExcelTable>(entity.RawData) : null;

            return result;
        }
    }
}
