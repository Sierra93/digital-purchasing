using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Extensions;
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
        private readonly INomenclatureService _nomenclatureService;
        private readonly IUploadedDocumentService _uploadedDocumentService;
        private readonly IUomService _uomService;
        private readonly ISupplierService _supplierService;

        public SupplierOfferService(
            ApplicationDbContext db,
            IExcelRequestReader excelRequestReader,
            IColumnNameService columnNameService,
            ICounterService counterService,
            ICurrencyService currencyService,
            ITenantService tenantService,
            INomenclatureService nomenclatureService,
            IUploadedDocumentService uploadedDocumentService,
            IUomService uomService,
            ISupplierService supplierService)
        {
            _db = db;
            _excelRequestReader = excelRequestReader;
            _columnNameService = columnNameService;
            _counterService = counterService;
            _currencyService = currencyService;
            _tenantService = tenantService;
            _nomenclatureService = nomenclatureService;
            _uploadedDocumentService = uploadedDocumentService;
            _uomService = uomService;
            _supplierService = supplierService;
        }

        public void UpdateStatus(Guid id, SupplierOfferStatus status)
        {
            var entity = _db.SupplierOffers.Find(id);
            entity.Status = status;
            _db.SaveChanges();
        }

        public void UpdateSupplierName(Guid id, string name, Guid? supplierId)
        {
            name = supplierId.HasValue
                ? _supplierService.GetNameById(supplierId.Value)
                : string.IsNullOrEmpty(name) ? null : name.Trim();
            var so = _db.SupplierOffers.Find(id);
            so.SupplierName = name;
            so.SupplierId = supplierId;
            _db.SaveChanges();
        }

        public void UpdateDeliveryCost(Guid id, decimal deliveryCost)
        {
            _db.SupplierOffers.Find(id).DeliveryCost = deliveryCost;
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
                .Include(q => q.Supplier)
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
            var offerItems = _db.SupplierOfferItems.Include(q => q.Nomenclature).ThenInclude(q => q.BatchUom).Where(q => q.SupplierOfferId == id).ToList();

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

                var offerItem = offerItems.FirstOrDefault(q => q.NomenclatureId.HasValue && q.NomenclatureId == requestItem.NomenclatureId);
                if (offerItem == null) continue;

                item.Offer.Code = offerItem.RawCode;
                item.Offer.Name = offerItem.RawName;
                item.Offer.Qty = offerItem.RawQty;
                item.Offer.Price = offerItem.RawPrice;
                item.Offer.Currency = supplierOffer.Currency.Name;
                item.Offer.Uom = offerItem.Nomenclature.BatchUom.Name;

                item.Mass.MassOf1 = requestItem.Nomenclature.MassUomValue;
                item.Mass.MassUom = requestItem.Nomenclature.MassUom.Name;

                item.ImportAndDelivery.DeliveryTerms = supplierOffer.DeliveryTerms;
                item.ImportAndDelivery.TotalDeliveryCost = supplierOffer.DeliveryCost;

                item.Conversion.CurrencyExchangeRate = 1; //TODO
                item.Conversion.UomRatio = 1; //TODO

                item.ResourceConversion.ResourceUom = requestItem.Nomenclature.ResourceUom.Name;
                item.ResourceConversion.ResourceBatchUom = requestItem.Nomenclature.ResourceBatchUom.Name;
                item.ResourceConversion.RequestResource = requestItem.Nomenclature.ResourceUomValue;
                item.ResourceConversion.OfferResource = requestItem.Nomenclature.ResourceUomValue; //TODO
                
                
            }

            return result;
        }

        public SupplierOfferColumnsDataVm GetColumnsData(Guid id)
        {
            var vm = GetById(id);
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

            var supplierOffer = _db.SupplierOffers.Include(q => q.UploadedDocument).ThenInclude(q => q.Headers).First(q => q.Id == id);

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

                rawItems.Add(rawItem);
            }

            _db.SupplierOfferItems.AddRange(rawItems);
            _db.SaveChanges();

            AutocompleteDataSOItems(id);
        }

        private void AutocompleteDataSOItems(Guid soId)
        {
            var so = _db.SupplierOffers.Find(soId);
            var soItems = _db.SupplierOfferItems.Where(q => q.SupplierOfferId == soId).ToList();

            string supplierName = null;
            if (soItems.Any())
            {
                _db.Entry(soItems.First()).Reference(q => q.SupplierOffer).Load();
                supplierName = soItems.First().SupplierOffer.SupplierName;
            }

            var uoms = new Dictionary<string, Guid>();
            var noms = new Dictionary<string, Guid>();

            foreach (var soItem in soItems)
            {
                #region Try to search in old PR items

                if (string.IsNullOrEmpty(soItem.RawCode))
                {
                    var otherRecords = _db.SupplierOfferItems
                        .Include(q => q.SupplierOffer)
                        .Where(q =>
                            q.RawName == soItem.RawName &&
                            q.SupplierOffer.SupplierName == supplierName &&
                            q.SupplierOfferId != soId &&
                            !string.IsNullOrEmpty(q.RawUomStr))
                        .ToList();

                    if (otherRecords.Any())
                    {
                        var rawUom = otherRecords.FirstOrDefault(q => !string.IsNullOrEmpty(q.RawUomStr))?.RawUomStr;
                        if (!string.IsNullOrEmpty(rawUom))
                        {
                            soItem.RawUomStr = rawUom;
                        }
                    }
                }

                #endregion

                #region Try to find UoM in db
                
                if (uoms.ContainsKey(soItem.RawUomStr))
                {
                    soItem.RawUomId = uoms[soItem.RawUomStr];
                }
                else
                {
                    var res = _uomService.Autocomplete(soItem.RawUomStr);
                    if (res.Items != null && res.Items.Any())
                    {
                        var match = res.Items.First();
                        if (match != null)
                        {
                            uoms.TryAdd(match.Name, match.Id);
                            soItem.RawUomId = match.Id;
                        }
                    }
                }

                #endregion
                #region Try to find in nomeclature
                
                if (noms.ContainsKey(soItem.RawName))
                {
                    soItem.NomenclatureId = noms[soItem.RawName];
                }
                else
                {
                    var nomRes = _nomenclatureService.Autocomplete(new AutocompleteOptions
                    {
                        Query = soItem.RawName,
                        ClientId = so.SupplierId.Value,
                        ClientType = ClientType.Supplier,
                        SearchInAlts = true
                    });

                    if (nomRes.Items != null && nomRes.Items.Count == 1)
                    {
                        noms.TryAdd(nomRes.Items[0].Name, nomRes.Items[0].Id);
                        soItem.NomenclatureId = nomRes.Items[0].Id;
                    }
                }

                #endregion

                #region Calc UoMs factor

                if (soItem.NomenclatureId != null && soItem.RawUomId != null)
                {
                    var rate = _uomService.GetConversionRate(soItem.RawUomId.Value, soItem.NomenclatureId.Value);
                    soItem.CommonFactor = rate.CommonFactor;
                    soItem.NomenclatureFactor = rate.NomenclatureFactor;
                }

                #endregion
            }

            _db.SaveChanges();

            foreach (var soItem in soItems)
            {
                _nomenclatureService.AddNomenclatureForSupplier(soItem.Id);
            }
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

        public void SaveMatch(Guid itemId, Guid nomenclatureId, Guid uomId, decimal factorC, decimal factorN)
        {
            var entity = _db.SupplierOfferItems.Find(itemId);
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
