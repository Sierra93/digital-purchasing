using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MatchItemsResponse = DigitalPurchasing.Core.Interfaces.MatchItemsResponse;
using PurchasingRequestColumns = DigitalPurchasing.Core.Interfaces.PurchasingRequestColumns;

namespace DigitalPurchasing.Services
{
    public class PurchasingRequestService : IPurchasingRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly IExcelRequestReader _excelRequestReader;
        private readonly IPRCounterService _counterService;
        private readonly IColumnNameService _columnNameService;

        public PurchasingRequestService(ApplicationDbContext db, IExcelRequestReader excelFileReader, IPRCounterService counterService, IColumnNameService columnNameService)
        {
            _db = db;
            _excelRequestReader = excelFileReader;
            _counterService = counterService;
            _columnNameService = columnNameService;
        }

        public CreateFromFileResponse CreateFromFile(string filePath)
        {
            var result = _excelRequestReader.ToTable(filePath);
            if (result == null || !result.IsSuccess) return new CreateFromFileResponse { IsSuccess = false, Message = result?.Message };

            var entry = _db.PurchasingRequests.Add(new PurchasingRequest
            {
                RawData = JsonConvert.SerializeObject(result.Table),
                Status = PurchasingRequestStatus.MatchColumns,
                PublicId = _counterService.GetNextId()
            });

            _db.SaveChanges();

            return new CreateFromFileResponse { IsSuccess = true, Id = entry.Entity.Id };
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

            // load from raw columns first
            var result = new PurchasingRequestColumnsResponse
            {
                Code = entity.RawColumns != null ? entity.RawColumns.Code : excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Code)?.Header ?? "",
                Name = entity.RawColumns != null ? entity.RawColumns.Name : excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Name)?.Header ?? "",
                Qty = entity.RawColumns != null ? entity.RawColumns.Qty : excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Qty)?.Header ?? "",
                Uom = entity.RawColumns != null ? entity.RawColumns.Uom : excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Uom)?.Header ?? "",
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

            if (!string.IsNullOrEmpty(purchasingRequestColumns.Name)) _columnNameService.SaveName(TableColumnType.Name, purchasingRequestColumns.Name);
            if (!string.IsNullOrEmpty(purchasingRequestColumns.Code)) _columnNameService.SaveName(TableColumnType.Code, purchasingRequestColumns.Code);
            if (!string.IsNullOrEmpty(purchasingRequestColumns.Qty)) _columnNameService.SaveName(TableColumnType.Qty, purchasingRequestColumns.Qty);
            if (!string.IsNullOrEmpty(purchasingRequestColumns.Uom)) _columnNameService.SaveName(TableColumnType.Uom, purchasingRequestColumns.Uom);
        }

        public void GenerateRawItems(Guid id)
        {
            _db.RemoveRange(_db.PurchasingRequestItems.Where(q => q.PurchasingRequestId == id));
            _db.SaveChanges();

            var entity = _db.PurchasingRequests.Include(q => q.RawColumns).First(q => q.Id == id);
            var table = JsonConvert.DeserializeObject<ExcelTable>(entity.RawData);
            var rawItems = new List<PurchasingRequestItem>();
            for (var i = 0; i < table.Columns.First(q => q.Type == TableColumnType.Name).Values.Count; i++)
            {
                var rawItem = new PurchasingRequestItem
                {
                    PurchasingRequestId = id,
                    Position = i + 1,
                    RawCode = string.IsNullOrEmpty(entity.RawColumns.Code) ? "" : table.GetValue(entity.RawColumns.Code, i),
                    RawName = string.IsNullOrEmpty(entity.RawColumns.Name) ? "" : table.GetValue(entity.RawColumns.Name, i),
                    RawQty = string.IsNullOrEmpty(entity.RawColumns.Qty) ? 0 : table.GetDecimalValue(entity.RawColumns.Qty, i),
                    RawUom = string.IsNullOrEmpty(entity.RawColumns.Uom) ? "" : table.GetValue(entity.RawColumns.Uom, i)
                };

                rawItems.Add(rawItem);
            }

            _db.PurchasingRequestItems.AddRange(rawItems);
            _db.SaveChanges();
        }

        public RawItemResponse GetRawItems(Guid id)
        {
            var items = _db.PurchasingRequestItems
                .Where(q => q.PurchasingRequestId == id)
                .OrderBy(q => q.Position)
                .ProjectToType<RawItemResponse.RawItem>()
                .ToList();

            return new RawItemResponse {Items = items};
        }

        public void SaveRawItems(Guid id, IEnumerable<RawItemResponse.RawItem> items)
        {
            _db.PurchasingRequestItems.RemoveRange(_db.PurchasingRequestItems.Where(q => q.PurchasingRequestId == id));
            _db.SaveChanges();
            var rawItems = items.Adapt<List<PurchasingRequestItem>>().ToList();
            var index = 0;
            foreach (var rawItem in rawItems)
            {
                rawItem.PurchasingRequestId = id;
                rawItem.Position = ++index;
            }
            _db.PurchasingRequestItems.AddRange(rawItems);
            _db.SaveChanges();
        }

        public void UpdateStatus(Guid id, PurchasingRequestStatus status)
        {
            var entity = _db.PurchasingRequests.Find(id);
            entity.Status = status;
            _db.SaveChanges();
        }

        public MatchItemsResponse MatchItemsData(Guid id)
        {
            var entity = _db.PurchasingRequestItems
                .Include(q => q.Nomenclature).ThenInclude(q => q.BatchUom)
                .Include(q => q.RawUomMatch)
                .Where(q => q.PurchasingRequestId == id)
                .OrderBy(q => q.Position).ToList();

            var res = new MatchItemsResponse {Items = entity.Adapt<List<MatchItemsResponse.Item>>()};

            return res;
        }

        public void SaveMatch(Guid itemId, Guid nomenclatureId, Guid uomId)
        {
            var entity = _db.PurchasingRequestItems.Find(itemId);
            entity.NomenclatureId = nomenclatureId;
            entity.RawUomMatchId = uomId;
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
