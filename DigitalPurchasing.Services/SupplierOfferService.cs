using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using EFCore.BulkExtensions;
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
        private readonly INomenclatureService _nomenclatureService;
        private readonly IUploadedDocumentService _uploadedDocumentService;
        private readonly IUomService _uomService;
        private readonly ISupplierService _supplierService;
        private readonly IConversionRateService _conversionRateService;
        private readonly IRootService _rootService;

        public SupplierOfferService(
            ApplicationDbContext db,
            IExcelRequestReader excelRequestReader,
            IColumnNameService columnNameService,
            ICounterService counterService,
            ICurrencyService currencyService,
            INomenclatureService nomenclatureService,
            IUploadedDocumentService uploadedDocumentService,
            IUomService uomService,
            ISupplierService supplierService,
            IConversionRateService conversionRateService,
            IRootService rootService)
        {
            _db = db;
            _excelRequestReader = excelRequestReader;
            _columnNameService = columnNameService;
            _counterService = counterService;
            _currencyService = currencyService;
            _nomenclatureService = nomenclatureService;
            _uploadedDocumentService = uploadedDocumentService;
            _uomService = uomService;
            _supplierService = supplierService;
            _conversionRateService = conversionRateService;
            _rootService = rootService;
        }

        public void UpdateStatus(Guid id, SupplierOfferStatus status, bool globalSearch = false)
        {
            var qry = _db.SupplierOffers.AsQueryable();
            if (globalSearch) qry = qry.IgnoreQueryFilters();

            var entity = qry.First(q => q.Id == id);
            entity.Status = status;
            _db.SaveChanges();
        }

        public void UpdateSupplierName(Guid id, string name, Guid? supplierId, bool globalSearch = false)
        {
            name = supplierId.HasValue
                ? _supplierService.GetNameById(supplierId.Value)
                : string.IsNullOrEmpty(name) ? null : name.Trim();

            var qry = _db.SupplierOffers.AsQueryable();
            if (globalSearch) qry = qry.IgnoreQueryFilters();

            var so = qry.First(q => q.Id == id);
            so.SupplierName = name;
            so.SupplierId = supplierId;
            _db.SaveChanges();
        }

        public void UpdateDeliveryCost(Guid id, decimal deliveryCost)
        {
            _db.SupplierOffers.Find(id).DeliveryCost = deliveryCost;
            _db.SaveChanges();
        }

        public async Task<CreateFromFileResponse> CreateFromFile(Guid competitionListId, string filePath)
        {
            var competitionList = _db.CompetitionLists.IgnoreQueryFilters().First(q => q.Id == competitionListId);
            var ownerId = competitionList.OwnerId;

            var result = _excelRequestReader.ToTable(filePath, ownerId);
            if (result == null || !result.IsSuccess) return new CreateFromFileResponse { IsSuccess = false, Message = result?.Message };
            
            var entry = await _db.SupplierOffers.AddAsync(new SupplierOffer
            {
                CompetitionListId = competitionListId,
                UploadedDocument = new UploadedDocument
                {
                    Data = JsonConvert.SerializeObject(result.Table),
                    Headers = new UploadedDocumentHeaders(),
                    OwnerId = ownerId,
                },
                Status = SupplierOfferStatus.MatchColumns,
                OwnerId = ownerId,
                PublicId = _counterService.GetSONextId(ownerId),
                CurrencyId = _currencyService.GetDefaultCurrency(ownerId).Id
            });

            await _db.SaveChangesAsync();

            var rootId = await _rootService.GetIdByCL(competitionListId);
            await _rootService.SetStatus(rootId, RootStatus.MatchingRequired);

            return new CreateFromFileResponse { Id = entry.Entity.Id, IsSuccess = true };
        }

        public SupplierOfferVm GetById(Guid id, bool globalSearch = false)
        {
            var qry = _db.SupplierOffers
                .Include(q => q.Supplier)
                .Include(q => q.UploadedDocument)
                .ThenInclude(q => q.Headers)
                .Include(q => q.CompetitionList)
                .ThenInclude(q => q.QuotationRequest)
                .AsQueryable();

            if (globalSearch) qry = qry.IgnoreQueryFilters();

            var entity = qry.FirstOrDefault(q => q.Id == id);
            
            var vm = entity?.Adapt<SupplierOfferVm>();
            if (vm != null)
            {
                vm.ExcelTable =  vm.UploadedDocument?.Data != null ? JsonConvert.DeserializeObject<ExcelTable>(vm.UploadedDocument?.Data) : null;
                vm.CompanyName = _db.PurchaseRequests
                    .IgnoreQueryFilters()
                    .First(q => q.Id == entity.CompetitionList.QuotationRequest.PurchaseRequestId).CompanyName;
                if (entity.Supplier != null)
                {
                    vm.SupplierName = entity.Supplier.Name;
                }
            }

            return vm;
        }

        public SupplierOfferDetailsVm GetDetailsById(Guid id)
        {
            var supplierOffer = _db.SupplierOffers
                .Include(q => q.Currency)
                .Include(q => q.CompetitionList)
                .ThenInclude(q => q.QuotationRequest)
                .First(q => q.Id == id);

            var purchaseRequestId = supplierOffer.CompetitionList.QuotationRequest.PurchaseRequestId;
            
            var requestItems = _db.PurchaseRequestItems
                .Include(q => q.Nomenclature)
                    .ThenInclude(q => q.BatchUom)
                .Include(q => q.Nomenclature.MassUom)
                .Include(q => q.Nomenclature.ResourceUom)
                .Include(q => q.Nomenclature.ResourceBatchUom)
                .Where(q => q.PurchaseRequestId == purchaseRequestId).ToList();
            var offerItems = _db.SupplierOfferItems
                .Include(q => q.RawUom)
                .Include(q => q.Nomenclature)
                    .ThenInclude(q => q.BatchUom)
                .Where(q => q.SupplierOfferId == id)
                .ToList();

            var result = new SupplierOfferDetailsVm
            {
                Id = id,
                PublicId = supplierOffer.PublicId,
                CreatedOn = supplierOffer.CreatedOn
            };

            foreach (var requestItem in requestItems)
            {
                var item = new SupplierOfferDetailsVm.Item(result.Items);
                result.Items.Add(item);

                item.Request.Code = requestItem.RawCode;
                item.Request.Name = requestItem.RawName;
                item.Request.Qty = requestItem.RawQty;
                item.Request.Currency = "RUB"; //TODO
                item.Request.Uom = requestItem.Nomenclature.BatchUom.Name;

                var offerItem = offerItems
                    .FirstOrDefault(q => q.NomenclatureId.HasValue && q.NomenclatureId == requestItem.NomenclatureId);
                if (offerItem == null) continue;

                item.Offer.Code = offerItem.RawCode;
                item.Offer.Name = offerItem.RawName;
                item.Offer.Qty = offerItem.RawQty;
                item.Offer.Price = offerItem.RawPrice;
                item.Offer.Currency = supplierOffer.Currency.Name;
                item.Offer.Uom = offerItem.RawUomId.HasValue ? offerItem.RawUom.Name : string.Empty;

                var factor = offerItem.CommonFactor > 0 ? offerItem.CommonFactor : offerItem.NomenclatureFactor;

                item.Mass.MassOf1 = requestItem.Nomenclature.MassUomValue; //todo: try to get from nom alt first
                item.Mass.MassUom = requestItem.Nomenclature.MassUom.Name;

                item.ImportAndDelivery.DeliveryTerms = supplierOffer.DeliveryTerms;
                item.ImportAndDelivery.TotalDeliveryCost = supplierOffer.DeliveryCost;

                item.Conversion.CurrencyExchangeRate = 1; //TODO
                item.Conversion.UomRatio = factor;

                item.ResourceConversion.ResourceUom = requestItem.Nomenclature.ResourceUom.Name;
                item.ResourceConversion.ResourceBatchUom = requestItem.Nomenclature.ResourceBatchUom.Name;
                item.ResourceConversion.RequestResource = requestItem.Nomenclature.ResourceUomValue;
                item.ResourceConversion.OfferResource = requestItem.Nomenclature.ResourceUomValue; //TODO
            }

            return result;
        }

        public SupplierOfferColumnsDataVm GetColumnsData(Guid id, bool globalSearch = false)
        {
            var vm = GetById(id, globalSearch);
            var result = new SupplierOfferColumnsDataVm
            {
                SupplierId = vm.SupplierId,
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

        public void SaveColumns(Guid supplierOfferId, SupplierOfferColumnsVm columns, bool globalSearch = false)
        {
            var qry = _db.SupplierOffers
                .Include(q => q.UploadedDocument)
                .ThenInclude(q => q.Headers)
                .AsQueryable();

            if (globalSearch) qry = qry.IgnoreQueryFilters();

            var entity = qry.First(q => q.Id == supplierOfferId);

            entity.UploadedDocument.Headers = columns.Adapt(entity.UploadedDocument.Headers);
            _db.SaveChanges();

            if (!string.IsNullOrEmpty(columns.Name)) _columnNameService.SaveName(TableColumnType.Name, columns.Name, entity.OwnerId);
            if (!string.IsNullOrEmpty(columns.Code)) _columnNameService.SaveName(TableColumnType.Code, columns.Code, entity.OwnerId);
            if (!string.IsNullOrEmpty(columns.Qty)) _columnNameService.SaveName(TableColumnType.Qty, columns.Qty, entity.OwnerId);
            if (!string.IsNullOrEmpty(columns.Uom)) _columnNameService.SaveName(TableColumnType.Uom, columns.Uom, entity.OwnerId);
            if (!string.IsNullOrEmpty(columns.Price)) _columnNameService.SaveName(TableColumnType.Price, columns.Price, entity.OwnerId);
        }

        public void GenerateRawItems(Guid id, bool globalSearch = false)
        {
            var qrySo = _db.SupplierOffers.AsQueryable();
            var qrySoItems = _db.SupplierOfferItems.AsQueryable();
            
            if (globalSearch)
            {
                qrySo = qrySo.IgnoreQueryFilters();
                qrySoItems = qrySoItems.IgnoreQueryFilters();
            }

            qrySoItems.Where(q => q.SupplierOfferId == id).BatchDelete();

            var supplierOffer = qrySo.Include(q => q.UploadedDocument).ThenInclude(q => q.Headers).First(q => q.Id == id);

            if (!supplierOffer.SupplierId.HasValue)
            {
                supplierOffer.SupplierId = _supplierService.CreateSupplier(supplierOffer.SupplierName);
            }

            var table = JsonConvert.DeserializeObject<ExcelTable>(supplierOffer.UploadedDocument.Data);
            var rawItems = new List<SupplierOfferItem>();
            for (var i = 0; i < table.Columns.First(q => q.Type == TableColumnType.Name).Values.Count; i++)
            {
                var rawItem = new SupplierOfferItem
                {
                    SupplierOfferId = id,
                    Position = i + 1, //todo: get from #, № and etc?
                    RawCode = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Code) ? "" : table.GetValue(supplierOffer.UploadedDocument.Headers.Code, i),
                    RawName = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Name) ? "" : table.GetValue(supplierOffer.UploadedDocument.Headers.Name, i),
                    RawQty = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Qty) ? 0 : table.GetDecimalValue(supplierOffer.UploadedDocument.Headers.Qty, i),
                    RawPrice = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Price) ? 0 : table.GetDecimalValue(supplierOffer.UploadedDocument.Headers.Price, i),
                    RawUomStr = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Uom) ? "" : table.GetValue(supplierOffer.UploadedDocument.Headers.Uom, i)
                };

                Autocomplete(supplierOffer, rawItem);

                rawItems.Add(rawItem);
            }

            _db.BulkInsert(rawItems);

            // todo: move to background job?
            _nomenclatureService.AddOrUpdateNomenclatureAlts(
                supplierOffer.OwnerId, supplierOffer.SupplierId.Value, ClientType.Supplier,
                rawItems
                    .Where(q => q.NomenclatureId.HasValue)
                    .Select(q => (
                        NomenclatureId: q.NomenclatureId.Value,
                        Name: q.RawName,
                        Code: q.RawCode,
                        Uom: q.RawUomId)
                    ).ToList());
        }

        private void Autocomplete(SupplierOffer so, SupplierOfferItem soItem)
        {
            #region Try to find UoM in db
                
            var res = _uomService.Autocomplete(soItem.RawUomStr, so.OwnerId);
            var fullMatch = res.Items?.FirstOrDefault(q => q.IsFullMatch);
            if (fullMatch != null)
            {
                soItem.RawUomId = fullMatch.Id;
            }

            #endregion
            #region Try to find in nomeclature
                
            var nomRes = _nomenclatureService.Autocomplete(new AutocompleteOptions
            {
                Query = soItem.RawName,
                ClientId = so.SupplierId.Value,
                ClientType = ClientType.Supplier,
                SearchInAlts = true,
                OwnerId = so.OwnerId
            });

            if (nomRes.Items != null && nomRes.Items.Count == 1)
            {
                soItem.NomenclatureId = nomRes.Items[0].Id;
            }

            #endregion

            #region Calc UoMs factor

            if (soItem.NomenclatureId != null && soItem.RawUomId != null)
            {
                var rate = _conversionRateService.GetRate(soItem.RawUomId.Value, soItem.NomenclatureId.Value);
                soItem.CommonFactor = rate.CommonFactor;
                soItem.NomenclatureFactor = rate.NomenclatureFactor;
            }

            #endregion
        }

        public SOMatchItemsVm MatchItemsData(Guid soId)
        {
            var supplierOffer = _db.SupplierOffers.Find(soId);
            if (supplierOffer == null)
            {
                return null;
            }

            var entities = _db.SupplierOfferItems
                .Include(q => q.Nomenclature).ThenInclude(q => q.BatchUom)
                .Include(q => q.RawUom)
                .Where(q => q.SupplierOfferId == soId)
                .OrderBy(q => q.Position).ToList();

            // Mappings - SupplierOfferItemMappings
            var res = new SOMatchItemsVm { Items = entities.Adapt<List<SOMatchItemsVm.Item>>(), SupplierName = supplierOffer.SupplierName };

            return res;
        }

        public void SaveMatch(Guid soItemId, Guid nomenclatureId, Guid uomId, decimal factorC, decimal factorN)
        {
            var entity = _db.SupplierOfferItems.Find(soItemId);
            entity.NomenclatureId = nomenclatureId;
            entity.RawUomId = uomId;
            entity.CommonFactor = factorC;
            entity.NomenclatureFactor = factorN;
            _db.SaveChanges();
        }

        public DeleteResultVm Delete(Guid id)
        {
            var so = _db.SupplierOffers.Find(id);
            if (so == null) return DeleteResultVm.Success();
            
            if (so.UploadedDocumentId.HasValue)
            {
                _uploadedDocumentService.Delete(so.UploadedDocumentId.Value);
            }

            _db.SupplierOfferItems.RemoveRange(_db.SupplierOfferItems.Where(q => q.SupplierOfferId == id));
            _db.SupplierOffers.Remove(so);
            _db.SaveChanges();

            return DeleteResultVm.Success();
        }

        public SoTermsVm GetTerms(Guid soId)
        {
            var so = _db.SupplierOffers.Find(soId);
            if (so == null) throw new ApplicationException("SO not found");

            var result = DefaultResult();
            result = so.Adapt(result);

            return result;
        }

        public void SaveTerms(SoTermsVm req, Guid soId)
        {
            var so = _db.SupplierOffers.Find(soId);
            if (so == null) throw new ApplicationException("SO not found");
            so = req.Adapt(so);
            if (so.PaymentTerms != PaymentTerms.Postponement)
            {
                so.PayWithinDays = 0;
            }
            _db.SaveChanges();
        }

        public bool IsAllMatchedBySoItem(Guid soItemId)
        {
            var item = _db.SupplierOfferItems.Find(soItemId);
            return IsAllMatched(item.SupplierOfferId);
        }

        public async Task<Guid> GetCLIdBySoItem(Guid soItemId)
        {
            var soItem = await _db.SupplierOfferItems
                .Include(q => q.SupplierOffer)
                .IgnoreQueryFilters()
                .FirstAsync(q => q.Id == soItemId);

            return soItem.SupplierOffer.CompetitionListId;
        }

        public bool IsAllMatched(Guid supplierOfferId)
        {
            var itemsQry = _db.SupplierOfferItems
                .IgnoreQueryFilters()
                .Where(q => q.SupplierOfferId == supplierOfferId);

            var totalCount = itemsQry.Count();

            var matchedQry = itemsQry
                .Where(q => q.NomenclatureId.HasValue
                            && ( q.CommonFactor > 0 || q.NomenclatureFactor > 0 )
                            && q.RawUomId.HasValue);

            var matchedCount = matchedQry.Count();

            return totalCount == matchedCount;
        }

        private SoTermsVm DefaultResult() => new SoTermsVm {
            DeliveryTermsOptions = GetDeliveryTermsOptions(),
            PaymentTermsOptions = GetPaymentTermsOptions(),
            ConfirmationDate = DateTime.UtcNow,
            DeliveryDate = DateTime.UtcNow
        };

        private IEnumerable<SoTermsVm.Option> GetPaymentTermsOptions()
            => Enum.GetValues(typeof(PaymentTerms)).OfType<PaymentTerms>().Select(value => new SoTermsVm.Option
            {
                Text = value.GetDescription(),
                Value = (int)value,
                Order = value.GetOrder()
            }).OrderBy(q => q.Order);

        private IEnumerable<SoTermsVm.Option> GetDeliveryTermsOptions()
            => Enum.GetValues(typeof(DeliveryTerms)).OfType<DeliveryTerms>().Select(value => new SoTermsVm.Option { Text = value.GetDescription(), Value = (int)value });
    }

    public class SupplierOfferItemMappings : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<SupplierOfferItem, SOMatchItemsVm.Item>()
                .Map(d => d.NomenclatureUom, s => s.NomenclatureId.HasValue ? s.Nomenclature.BatchUom.Name : null)
                .Map(q => q.RawUomStr, q => q.RawUomId.HasValue ? q.RawUom.Name : q.RawUomStr);

            config.NewConfig<SupplierOfferItem, CompetitionListVm.SupplierOfferItemVm>()
                .Map(q => q.RawUom, q => q.RawUomId.HasValue ? q.RawUom.Name : q.RawUomStr);
        }
    }
}
