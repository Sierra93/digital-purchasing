using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public SupplierOfferService(
            ApplicationDbContext db,
            IExcelRequestReader excelRequestReader,
            IColumnNameService columnNameService)
        {
            _db = db;
            _excelRequestReader = excelRequestReader;
            _columnNameService = columnNameService;
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
                Status = SupplierOfferStatus.MatchColumns
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
                .FirstOrDefault(q => q.Id == id);
            
            var vm = entity?.Adapt<SupplierOfferVm>();
            if (vm != null)
            {
                vm.ExcelTable =  vm.UploadedDocument?.Data != null ? JsonConvert.DeserializeObject<ExcelTable>(vm.UploadedDocument?.Data) : null;
            }
            return vm;
        }

        public SupplierOfferColumnsDataVm GetColumnsData(Guid id)
        {
            var vm = GetById(id);
            var result = new SupplierOfferColumnsDataVm
            {
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
    }
}
