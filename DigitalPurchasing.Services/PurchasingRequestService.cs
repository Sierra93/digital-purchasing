using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PurchasingRequestColumns = DigitalPurchasing.Core.Interfaces.PurchasingRequestColumns;

namespace DigitalPurchasing.Services
{
    public class PurchasingRequestService : IPurchasingRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly IExcelRequestReader _excelRequestReader;
        private readonly IPRCounterService _counterService;

        public PurchasingRequestService(ApplicationDbContext db, IExcelRequestReader excelFileReader, IPRCounterService counterService)
        {
            _db = db;
            _excelRequestReader = excelFileReader;
            _counterService = counterService;
        }

        public Guid CreateFromFile(string filePath)
        {
            var table = _excelRequestReader.ToTable(filePath);
            var entry = _db.PurchasingRequests.Add(new PurchasingRequest
            {
                RawData = JsonConvert.SerializeObject(table),
                Type = PurchasingRequestType.AppropriateColumns,
                PublicId = _counterService.GetNextId()
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

        public PurchasingRequestColumnsResponse GetColumnsById(Guid id)
        {
            var entity = _db.PurchasingRequests.Include(q => q.RawColumns).First(q => q.Id == id);
            var excelTable = JsonConvert.DeserializeObject<ExcelTable>(entity.RawData);
            var result = new PurchasingRequestColumnsResponse
            {
                Id = excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Id)?.Header ?? "",
                Code = excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Code)?.Header ?? "",
                Name = excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Name)?.Header ?? "",
                Qty = excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Qty)?.Header ?? "",
                Uom = excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Uom)?.Header ?? "",
                Date = excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Date)?.Header ?? "",
                Receiver = excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Receiver)?.Header ?? "",
                Columns = excelTable.Columns.Select(q => q.Header).ToList(),
                IsSaved = entity.RawColumns != null 
            };
            return result;
        }

        public void SaveColumns(Guid id, PurchasingRequestColumns purchasingRequestColumns)
        {
            var entity = _db.PurchasingRequests.Include(q => q.RawColumns).First(q => q.Id == id);
            if (entity.RawColumns == null)
            {
                entity.RawColumns = new RawColumns();
            }

            entity.RawColumns = purchasingRequestColumns.Adapt(entity.RawColumns);
            _db.SaveChanges();
        }

        public PurchasingRequestDataResponse GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Id";
            }

            var qry =  _db.PurchasingRequests.AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<PurchasingRequestData>().ToList();
            return new PurchasingRequestDataResponse
            {
                Total = total,
                Data = result
            };
        }
    }
}
