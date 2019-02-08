using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core;
using DigitalPurchasing.Models;
using EFCore.BulkExtensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DigitalPurchasing.Services
{
    public class PurchaseRequestService : IPurchaseRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly IExcelRequestReader _excelRequestReader;
        private readonly ICounterService _counterService;
        private readonly IColumnNameService _columnNameService;
        private readonly IUomService _uomService;
        private readonly INomenclatureService _nomenclatureService;
        private readonly IDeliveryService _deliveryService;
        private readonly IUploadedDocumentService _uploadedDocumentService;
        private readonly ICustomerService _customerService;

        public PurchaseRequestService(
            ApplicationDbContext db,
            IExcelRequestReader excelFileReader,
            ICounterService counterService,
            IColumnNameService columnNameService,
            IUomService uomService,
            INomenclatureService nomenclatureService,
            IDeliveryService deliveryService,
            IUploadedDocumentService uploadedDocumentService,
            ICustomerService customerService)
        {
            _db = db;
            _excelRequestReader = excelFileReader;
            _counterService = counterService;
            _columnNameService = columnNameService;
            _uomService = uomService;
            _nomenclatureService = nomenclatureService;
            _deliveryService = deliveryService;
            _uploadedDocumentService = uploadedDocumentService;
            _customerService = customerService;
        }

        public CreateFromFileResponse CreateFromFile(string filePath)
        {
            var result = _excelRequestReader.ToTable(filePath);
            if (result == null || !result.IsSuccess) return new CreateFromFileResponse { IsSuccess = false, Message = result?.Message };

            var entry = _db.PurchaseRequests.Add(new PurchaseRequest
            {
                UploadedDocument = new UploadedDocument
                {
                    Data = JsonConvert.SerializeObject(result.Table),
                    Headers = new UploadedDocumentHeaders()
                },
                Status = PurchaseRequestStatus.MatchColumns,
                PublicId = _counterService.GetPRNextId()
            });

            _db.SaveChanges();

            return new CreateFromFileResponse { IsSuccess = true, Id = entry.Entity.Id };
        }

        public PurchaseRequestDetailsResponse GetById(Guid id)
        {
            var entity = _db.PurchaseRequests.Include(q => q.UploadedDocument).First(q => q.Id == id);

            var result = entity.Adapt<PurchaseRequestDetailsResponse>();
            result.ExcelTable =
                entity.UploadedDocument.Data != null ? JsonConvert.DeserializeObject<ExcelTable>(entity.UploadedDocument.Data) : null;

            return result;
        }

        public PurchaseRequestColumnsResponse GetColumnsById(Guid id)
        {
            var entity = _db.PurchaseRequests.Include(q => q.UploadedDocument).ThenInclude(q => q.Headers).First(q => q.Id == id);
            var excelTable = JsonConvert.DeserializeObject<ExcelTable>(entity.UploadedDocument.Data);

            // load from raw columns first
            var result = new PurchaseRequestColumnsResponse
            {
                Code = entity.UploadedDocument?.Headers?.Code ?? (excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Code)?.Header ?? ""),
                Name = entity.UploadedDocument?.Headers?.Name ?? (excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Name)?.Header ?? ""),
                Qty = entity.UploadedDocument?.Headers?.Qty ?? (excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Qty)?.Header ?? ""),
                Uom = entity.UploadedDocument?.Headers?.Uom ?? (excelTable.Columns.FirstOrDefault(q => q.Type == TableColumnType.Uom)?.Header ?? ""),
                Columns = excelTable.Columns.Select(q => q.Header).ToList(),
                IsSaved = entity.UploadedDocument != null 
            };

            return result;
        }

        public void SaveColumns(Guid id, PurchaseRequestColumns purchasingRequestColumns)
        {
            var entity = _db.PurchaseRequests.Include(q => q.UploadedDocument).ThenInclude(q => q.Headers).First(q => q.Id == id);
            if (entity.UploadedDocument == null)
            {
                entity.UploadedDocument = new UploadedDocument();
            }

            entity.UploadedDocument.Headers = purchasingRequestColumns.Adapt(entity.UploadedDocument.Headers);
            _db.SaveChanges();

            if (!string.IsNullOrEmpty(purchasingRequestColumns.Name)) _columnNameService.SaveName(TableColumnType.Name, purchasingRequestColumns.Name);
            if (!string.IsNullOrEmpty(purchasingRequestColumns.Code)) _columnNameService.SaveName(TableColumnType.Code, purchasingRequestColumns.Code);
            if (!string.IsNullOrEmpty(purchasingRequestColumns.Qty)) _columnNameService.SaveName(TableColumnType.Qty, purchasingRequestColumns.Qty);
            if (!string.IsNullOrEmpty(purchasingRequestColumns.Uom)) _columnNameService.SaveName(TableColumnType.Uom, purchasingRequestColumns.Uom);
        }

        public void GenerateRawItems(Guid id)
        {
            _db.RemoveRange(_db.PurchaseRequestItems.Where(q => q.PurchaseRequestId == id));
            _db.SaveChanges();

            var entity = _db.PurchaseRequests.Include(q => q.UploadedDocument).ThenInclude(q => q.Headers).First(q => q.Id == id);
            var table = JsonConvert.DeserializeObject<ExcelTable>(entity.UploadedDocument.Data);
            var rawItems = new List<PurchaseRequestItem>();
            for (var i = 0; i < table.Columns.First(q => q.Type == TableColumnType.Name).Values.Count; i++)
            {
                var rawItem = new PurchaseRequestItem
                {
                    PurchaseRequestId = id,
                    Position = i + 1,
                    RawCode = string.IsNullOrEmpty(entity.UploadedDocument.Headers.Code) ? "" : table.GetValue(entity.UploadedDocument.Headers.Code, i),
                    RawName = string.IsNullOrEmpty(entity.UploadedDocument.Headers.Name) ? "" : table.GetValue(entity.UploadedDocument.Headers.Name, i),
                    RawQty = string.IsNullOrEmpty(entity.UploadedDocument.Headers.Qty) ? 0 : table.GetDecimalValue(entity.UploadedDocument.Headers.Qty, i),
                    RawUom = string.IsNullOrEmpty(entity.UploadedDocument.Headers.Uom) ? "" : table.GetValue(entity.UploadedDocument.Headers.Uom, i)
                };

                rawItems.Add(rawItem);
            }

            _db.PurchaseRequestItems.AddRange(rawItems);
            _db.SaveChanges();
        }

        public RawItemResponse GetRawItems(Guid id)
        {
            var pr = _db.PurchaseRequests.Find(id);
            if (pr == null)
            {
                return null;
            }

            var items = _db.PurchaseRequestItems
                .Where(q => q.PurchaseRequestId == id)
                .OrderBy(q => q.Position)
                .ProjectToType<RawItemResponse.RawItem>()
                .ToList();

            return new RawItemResponse { Items = items, CustomerName = pr.CustomerName };
        }

        public void SaveRawItems(Guid id, IEnumerable<RawItemResponse.RawItem> items)
        {
            var pr = _db.PurchaseRequests.Find(id);
            if (pr == null) return;

            if (!pr.CustomerId.HasValue)
            {
                pr.CustomerId = _customerService.CreateCustomer(pr.CustomerName);
            }

            _db.PurchaseRequestItems.Where(q => q.PurchaseRequestId == id).BatchDelete();

            var prItems = items.Adapt<List<PurchaseRequestItem>>().ToList();
            var index = 0;

            foreach (var prItem in prItems)
            {
                prItem.PurchaseRequestId = id;
                prItem.Position = ++index;
            }

            _db.BulkInsert(prItems);

            AutocompleteDataPRItems(id);
        }

        private void AutocompleteDataPRItems(Guid prId)
        {
            var pr = _db.PurchaseRequests.Find(prId);
            var prItems = _db.PurchaseRequestItems.Where(q => q.PurchaseRequestId == prId).ToList();

            var customerName = pr.CustomerName;

            var uoms = new Dictionary<string, Guid>();
            var noms = new Dictionary<string, Guid>();

            foreach (var prItem in prItems)
            {
                #region Try to search in old PR items

                if (string.IsNullOrEmpty(prItem.RawUom))
                {
                    var otherRecords = _db.PurchaseRequestItems
                        .Include(q => q.PurchaseRequest)
                        .Where(q =>
                            q.RawName == prItem.RawName &&
                            q.PurchaseRequest.CustomerName == customerName &&
                            q.PurchaseRequestId != prId &&
                            !string.IsNullOrEmpty(q.RawUom))
                        .ToList();

                    if (otherRecords.Any())
                    {
                        var rawUom = otherRecords.FirstOrDefault(q => !string.IsNullOrEmpty(q.RawUom))?.RawUom;
                        if (!string.IsNullOrEmpty(rawUom))
                        {
                            prItem.RawUom = rawUom;
                        }
                    }
                }

                #endregion

                #region Try to find UoM in db
                
                if (uoms.ContainsKey(prItem.RawUom))
                {
                    prItem.RawUomMatchId = uoms[prItem.RawUom];
                }
                else
                {
                    var res = _uomService.Autocomplete(prItem.RawUom);
                    if (res.Items != null && res.Items.Any())
                    {
                        var match = res.Items.First();
                        if (match != null)
                        {
                            uoms.TryAdd(match.Name, match.Id);
                            prItem.RawUomMatchId = match.Id;
                        }
                    }
                }

                #endregion
                #region Try to find in nomeclature
                
                if (noms.ContainsKey(prItem.RawName))
                {
                    prItem.NomenclatureId = noms[prItem.RawName];
                }
                else
                {
                    var nomRes = _nomenclatureService.Autocomplete(new AutocompleteOptions
                    {
                        Query = prItem.RawName,
                        ClientId = pr.CustomerId.Value,
                        ClientType = ClientType.Customer,
                        SearchInAlts = true
                    });
                    if (nomRes.Items != null && nomRes.Items.Count == 1)
                    {
                        noms.TryAdd(nomRes.Items[0].Name, nomRes.Items[0].Id);
                        prItem.NomenclatureId = nomRes.Items[0].Id;
                    }
                }

                #endregion

                #region Calc UoMs factor

                if (prItem.NomenclatureId != null && prItem.RawUomMatchId != null)
                {
                    var rate = _uomService.GetConversionRate(prItem.RawUomMatchId.Value, prItem.NomenclatureId.Value);
                    prItem.CommonFactor = rate.CommonFactor;
                    prItem.NomenclatureFactor = rate.NomenclatureFactor;
                }

                #endregion
            }

            _db.BulkUpdate(prItems);
        }

        public void UpdateStatus(Guid id, PurchaseRequestStatus status)
        {
            var entity = _db.PurchaseRequests.Find(id);
            entity.Status = status;
            _db.SaveChanges();
        }

        public PRMatchItemsResponse MatchItemsData(Guid id)
        {
            var pr = _db.PurchaseRequests.Find(id);
            if (pr == null)
            {
                return null;
            }

            var entities = _db.PurchaseRequestItems
                .Include(q => q.Nomenclature).ThenInclude(q => q.BatchUom)
                .Include(q => q.RawUomMatch)
                .Where(q => q.PurchaseRequestId == id)
                .OrderBy(q => q.Position).ToList();

            // mappings - PurchaseRequestItemMappings
            var res = new PRMatchItemsResponse { Items = entities.Adapt<List<PRMatchItemsResponse.Item>>(), CustomerName = pr.CustomerName };

            return res;
        }

        public void SaveMatch(Guid itemId, Guid nomenclatureId, Guid uomId, decimal factorC, decimal factorN)
        {
            var entity = _db.PurchaseRequestItems.Find(itemId);
            entity.NomenclatureId = nomenclatureId;
            entity.RawUomMatchId = uomId;
            entity.CommonFactor = factorC;
            entity.NomenclatureFactor = factorN;
            _db.SaveChanges();
        }

        public void SaveCompanyName(Guid prId, string companyName)
        {
            var entity = _db.PurchaseRequests.Find(prId);
            entity.CompanyName = companyName;
            _db.SaveChanges();
        }

        public void SaveCustomerName(Guid id, string name, Guid? customerId)
        {
            name = customerId.HasValue
                ? _customerService.GetNameById(customerId.Value)
                : string.IsNullOrEmpty(name) ? null : name.Trim();
            var so = _db.PurchaseRequests.Find(id);
            so.CustomerName = name;
            so.CustomerId = customerId;
            _db.SaveChanges();
        }

        public DeleteResultVm Delete(Guid id)
        {
            var qr = _db.QuotationRequests.First(q => q.PurchaseRequestId == id);
            if (qr != null)
            {
                return DeleteResultVm.Failure($"Нельзя удалить заявку, удалите сперва запрос предложения №{qr.PublicId}");
            }

            var pr = _db.PurchaseRequests.Find(id);
            if (pr == null) return DeleteResultVm.Success();

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (pr.UploadedDocumentId.HasValue)
                    {
                        _uploadedDocumentService.Delete(pr.UploadedDocumentId.Value);
                    }
                    if (pr.DeliveryId.HasValue)
                    {
                        _deliveryService.Delete(pr.DeliveryId.Value);
                    }
                    _db.PurchaseRequestItems.RemoveRange(_db.PurchaseRequestItems.Where(q => q.PurchaseRequestId == id));
                    _db.PurchaseRequests.Remove(pr);
                    _db.SaveChanges();
                    transaction.Commit();
                    
                    return new DeleteResultVm(true, null);
                }
                catch (Exception e)
                {
                    //todo: log e
                    transaction.Rollback();
                    return DeleteResultVm.Failure("Внутренняя ошибка. Обратитесь в службу поддержки");
                }
            }
        }

        public PurchaseRequestIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Id";
            }

            var qry =  _db.PurchaseRequests.AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<PurchasingRequestIndexDataItem>().ToList();
            return new PurchaseRequestIndexData
            {
                Total = total,
                Data = result
            };
        }
    }

    public class PurchaseRequestItemMappings : IRegister
    {
        public void Register(TypeAdapterConfig config) =>
            config.NewConfig<PurchaseRequestItem, PRMatchItemsResponse.Item>()
                .Map(d => d.NomenclatureUom, s => s.NomenclatureId.HasValue ? s.Nomenclature.BatchUom.Name : null)
                .Map(q => q.RawUom, q => q.RawUomMatchId.HasValue ? q.RawUomMatch.Name : q.RawUom);
    }
}
