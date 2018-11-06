using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DigitalPurchasing.Services
{
    public class SupplierOfferService : ISupplierOfferService
    {
        private readonly ApplicationDbContext _db;
        private readonly IExcelRequestReader _excelRequestReader;
        private readonly IColumnNameService _columnNameService;
        private readonly ICounterService _counterService;
        private readonly ICurrencyService _currencyService;
        private readonly ITenantService _tenantService;

        public SupplierOfferService(
            ApplicationDbContext db,
            IExcelRequestReader excelRequestReader,
            IColumnNameService columnNameService,
            ICounterService counterService,
            ICurrencyService currencyService,
            ITenantService tenantService)
        {
            _db = db;
            _excelRequestReader = excelRequestReader;
            _columnNameService = columnNameService;
            _counterService = counterService;
            _currencyService = currencyService;
            _tenantService = tenantService;
        }

        public void UpdateStatus(Guid id, SupplierOfferStatus status)
        {
            var entity = _db.SupplierOffers.Find(id);
            entity.Status = status;
            _db.SaveChanges();
        }

        public void UpdateSupplierName(Guid id, string name)
        {
            _db.SupplierOffers.Find(id).SupplierName = name;
            _db.SaveChanges();
        }

        public CreateFromFileResponse CreateFromFile(Guid competitionListId, string filePath)
        {
            var result = _excelRequestReader.ToTable(filePath);
            if (result == null || !result.IsSuccess) return new CreateFromFileResponse { IsSuccess = false, Message = result?.Message };

            var entry = _db.SupplierOffers.Add(new SupplierOffer
            {
                CompetitionListId = competitionListId,
                UploadedDocument = new UploadedDocument
                {
                    Data = JsonConvert.SerializeObject(result.Table),
                    Headers = new UploadedDocumentHeaders()
                },
                Status = SupplierOfferStatus.MatchColumns,
                PublicId = _counterService.GetSONextId(),
                CurrencyId = _currencyService.GetDefaultCurrency(_tenantService.Get().CompanyId).Id
            });

            _db.SaveChanges();

            return new CreateFromFileResponse { Id = entry.Entity.Id, IsSuccess = true };
        }

        public SupplierOfferVm GetById(Guid id)
        {
            var entity = _db.SupplierOffers
                .Include(q => q.UploadedDocument)
                    .ThenInclude(q => q.Headers)
                .Include(q => q.CompetitionList)
                    .ThenInclude(q => q.QuotationRequest)
                .FirstOrDefault(q => q.Id == id);
            
            var vm = entity?.Adapt<SupplierOfferVm>();
            if (vm != null)
            {
                vm.ExcelTable =  vm.UploadedDocument?.Data != null ? JsonConvert.DeserializeObject<ExcelTable>(vm.UploadedDocument?.Data) : null;
                vm.CompanyName = _db.PurchaseRequests.Find(entity.CompetitionList.QuotationRequest.PurchaseRequestId).CompanyName;
            }

            return vm;
        }

        public SupplierOfferColumnsDataVm GetColumnsData(Guid id)
        {
            var vm = GetById(id);
            var result = new SupplierOfferColumnsDataVm
            {
                SupplierName = vm.SupplierName,
                Code = vm.UploadedDocument?.Headers?.Code ?? (vm.ExcelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Code)?.Header ?? ""),
                Name = vm.UploadedDocument?.Headers?.Name ?? (vm.ExcelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Name)?.Header ?? ""),
                Qty = vm.UploadedDocument?.Headers?.Qty ?? (vm.ExcelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Qty)?.Header ?? ""),
                Uom = vm.UploadedDocument?.Headers?.Uom ?? (vm.ExcelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Uom)?.Header ?? ""),
                Price = vm.UploadedDocument?.Headers?.Price ?? (vm.ExcelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Price)?.Header ?? ""),
                Columns = vm.ExcelTable.Columns.Select(q => q.Header).ToList()
            };
            return result;
        }

        public void SaveColumns(Guid supplierOfferId, SupplierOfferColumnsVm columns)
        {
            var entity = _db.SupplierOffers.Include(q => q.UploadedDocument).ThenInclude(q => q.Headers).First(q => q.Id == supplierOfferId);

            entity.UploadedDocument.Headers = columns.Adapt(entity.UploadedDocument.Headers);
            _db.SaveChanges();

            if (!string.IsNullOrEmpty(columns.Name)) _columnNameService.SaveName(TableColumnType.Name, columns.Name);
            if (!string.IsNullOrEmpty(columns.Code)) _columnNameService.SaveName(TableColumnType.Code, columns.Code);
            if (!string.IsNullOrEmpty(columns.Qty)) _columnNameService.SaveName(TableColumnType.Qty, columns.Qty);
            if (!string.IsNullOrEmpty(columns.Uom)) _columnNameService.SaveName(TableColumnType.Uom, columns.Uom);
            if (!string.IsNullOrEmpty(columns.Price)) _columnNameService.SaveName(TableColumnType.Price, columns.Price);
        }

        public void GenerateRawItems(Guid id)
        {
            _db.RemoveRange(_db.SupplierOfferItems.Where(q => q.SupplierOfferId == id));
            _db.SaveChanges();

            var entity = _db.SupplierOffers.Include(q => q.UploadedDocument).ThenInclude(q => q.Headers).First(q => q.Id == id);
            var table = JsonConvert.DeserializeObject<ExcelTable>(entity.UploadedDocument.Data);
            var rawItems = new List<SupplierOfferItem>();
            for (var i = 0; i < table.Columns.First(q => q.Type == TableColumnType.Name).Values.Count; i++)
            {
                var rawItem = new SupplierOfferItem
                {
                    SupplierOfferId = id,
                    Position = i + 1, //todo: get from #, № and etc?
                    RawCode = string.IsNullOrEmpty(entity.UploadedDocument.Headers.Code) ? "" : table.GetValue(entity.UploadedDocument.Headers.Code, i),
                    RawName = string.IsNullOrEmpty(entity.UploadedDocument.Headers.Name) ? "" : table.GetValue(entity.UploadedDocument.Headers.Name, i),
                    RawQty = string.IsNullOrEmpty(entity.UploadedDocument.Headers.Qty) ? 0 : table.GetDecimalValue(entity.UploadedDocument.Headers.Qty, i),
                    RawPrice = string.IsNullOrEmpty(entity.UploadedDocument.Headers.Price) ? 0 : table.GetDecimalValue(entity.UploadedDocument.Headers.Price, i),
                    RawUomStr = string.IsNullOrEmpty(entity.UploadedDocument.Headers.Uom) ? "" : table.GetValue(entity.UploadedDocument.Headers.Uom, i)
                };

                rawItems.Add(rawItem);
            }

            _db.SupplierOfferItems.AddRange(rawItems);
            _db.SaveChanges();
        }

        public SOMatchItemsVm MatchItemsData(Guid soId)
        {
            var supplierOffer = _db.SupplierOffers.Find(soId);
            if (supplierOffer == null)
            {
                return null;
            }

            var entities = _db.SupplierOfferItems
                //.Include(q => q.Nomenclature).ThenInclude(q => q.BatchUom)
                //.Include(q => q.RawUomMatch)
                .Where(q => q.SupplierOfferId == soId)
                .OrderBy(q => q.Position).ToList();

            var res = new SOMatchItemsVm { Items = entities.Adapt<List<SOMatchItemsVm.Item>>(), SupplierName = supplierOffer.SupplierName };

            return res;
        }
    }
}
