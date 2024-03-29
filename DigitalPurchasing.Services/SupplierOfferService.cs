using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using EFCore.BulkExtensions;
using F23.StringSimilarity;
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
        private readonly INomenclatureAlternativeService _nomenclatureAlternativeService;
        private readonly INomenclatureComparisonService _nomenclatureComparisonService;

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
            IRootService rootService,
            INomenclatureAlternativeService nomenclatureAlternativeService,
            INomenclatureComparisonService nomenclatureComparisonService)
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
            _nomenclatureAlternativeService = nomenclatureAlternativeService;
            _nomenclatureComparisonService = nomenclatureComparisonService;
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

        public void UpdateContactPerson(Guid supplierOfferId, Guid? contactPersonId)
        {
            _db.SupplierOffers.Find(supplierOfferId).ContactPersonId = contactPersonId;
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
                CurrencyId = _currencyService.GetDefaultCurrency(ownerId).Id,
                InvoiceData = result.InvoiceData,
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
                .AsQueryable();

            var rootsQry = _db.Roots.AsQueryable();

            if (globalSearch)
            {
                qry = qry.IgnoreQueryFilters();
                rootsQry = rootsQry.IgnoreQueryFilters();
            }

            var entity = qry.FirstOrDefault(q => q.Id == id);

            var root = rootsQry.First(q => q.CompetitionListId == entity.CompetitionListId);
            
            var vm = entity?.Adapt<SupplierOfferVm>();
            if (vm != null)
            {
                vm.PurchaseRequestId = root.PurchaseRequestId ?? Guid.Empty;
                vm.CompetitionListId = root.CompetitionListId ?? Guid.Empty;
                vm.ExcelTable = vm.UploadedDocument?.Data != null ? JsonConvert.DeserializeObject<ExcelTable>(vm.UploadedDocument?.Data) : null;
                var pr = _db.PurchaseRequests
                    .IgnoreQueryFilters()
                    .First(q => q.Id == vm.PurchaseRequestId);
                vm.CompanyName = pr.CompanyName;

                if (entity.Supplier != null)
                {
                    vm.SupplierName = entity.Supplier.Name;
                }
            }

            return vm;
        }

        public SupplierOfferDetailsVm GetDetailsById(Guid id, bool globalSearch = false)
        {
            var supplierOffersQry = _db.SupplierOffers
                .Include(q => q.Currency)
                .Include(q => q.Supplier)
                .Include(q => q.CompetitionList)
                    .ThenInclude(q => q.QuotationRequest)
                .AsNoTracking()
                .AsQueryable();

            var supplierOfferItemsQry = _db.SupplierOfferItems
                .Include(q => q.RawUom)
                .Include(q => q.Nomenclature)
                    .ThenInclude(q => q.BatchUom)
                .AsNoTracking()
                .AsQueryable();

            var purchaseRequestItemsQry = _db.PurchaseRequestItems
                .Include(q => q.Nomenclature)
                    .ThenInclude(q => q.BatchUom)
                .Include(q => q.Nomenclature.MassUom)
                .Include(q => q.Nomenclature.ResourceUom)
                .Include(q => q.Nomenclature.ResourceBatchUom)
                .AsNoTracking()
                .AsQueryable();

            var nomenclatureAlternativesQry = _db.NomenclatureAlternatives
                .Include(q => q.Link)
                .AsNoTracking()
                .AsQueryable();

            if (globalSearch)
            {
                supplierOffersQry = supplierOffersQry.IgnoreQueryFilters();
                supplierOfferItemsQry = supplierOfferItemsQry.IgnoreQueryFilters();
                purchaseRequestItemsQry = purchaseRequestItemsQry.IgnoreQueryFilters();
                nomenclatureAlternativesQry = nomenclatureAlternativesQry.IgnoreQueryFilters();
            }

            var supplierOffer = supplierOffersQry
                .First(q => q.Id == id);

            var purchaseRequestId = supplierOffer.CompetitionList.QuotationRequest.PurchaseRequestId;
            
            var requestItems = purchaseRequestItemsQry
                .Where(q => q.PurchaseRequestId == purchaseRequestId)
                .ToList();

            var offerItems = supplierOfferItemsQry
                .Where(q => q.SupplierOfferId == id)
                .ToList();

            var nomIds = requestItems
                .Select(q => q.Nomenclature.Id)
                .ToList();

            var nomAlts = nomenclatureAlternativesQry
                .Where(q => q.Link.SupplierId == supplierOffer.SupplierId && nomIds.Contains(q.NomenclatureId))
                .ToList();

            var result = new SupplierOfferDetailsVm
            {
                Id = id,
                PublicId = supplierOffer.PublicId,
                CreatedOn = supplierOffer.CreatedOn,
                SupplierId = supplierOffer.SupplierId,
                DeliveryTerms = supplierOffer.DeliveryTerms,
                PaymentTerms = supplierOffer.PaymentTerms,
                PayWithinDays = supplierOffer.PayWithinDays,
                DeliveryDate = supplierOffer.DeliveryDate,
                SupplierName = supplierOffer.Supplier?.Name,
                InvoiceData = supplierOffer.InvoiceData
            };

            foreach (var requestItem in requestItems)
            {
                var item = new SupplierOfferDetailsVm.Item(result.Items);
                result.Items.Add(item);

                item.NomenclatureId = requestItem.Nomenclature.Id;
                item.Position = requestItem.Position;
                item.Request.ItemId = requestItem.Id;
                item.Request.Code = requestItem.RawCode;
                item.Request.Name = requestItem.RawName;
                item.Request.Qty = requestItem.RawQty;

                var qtyMod = requestItem.Nomenclature.BatchUom.Quantity ?? 1;

                item.Request.QtyMod = qtyMod;
                item.Request.Currency = "RUB"; //TODO
                item.Request.Uom = requestItem.Nomenclature.BatchUom.Name;

                var offerItem = FindOfferItem(offerItems, requestItem);
                if (offerItem == null) continue;

                item.Offer.ItemId = offerItem.Id;
                item.Offer.Code = offerItem.RawCode;
                item.Offer.Name = offerItem.RawName;
                item.Offer.Qty = offerItem.RawQty;
                item.Offer.Price = offerItem.RawPrice;
                item.Offer.Currency = supplierOffer.Currency.Name;
                item.Offer.Uom = offerItem.RawUomId.HasValue ? offerItem.RawUom.Name : string.Empty;

                var nomAlt = nomAlts.FirstOrDefault(q => q.NomenclatureId == requestItem.Nomenclature.Id);

                decimal offerMassOf1;

                var factor = offerItem.CommonFactor > 0 ? offerItem.CommonFactor : offerItem.NomenclatureFactor;

                if (offerItem.RawUomId.HasValue && requestItem.Nomenclature.MassUomId == offerItem.RawUomId.Value)
                {
                    offerMassOf1 = 1;
                }
                else
                {
                    offerMassOf1 = (nomAlt?.MassUomValue > 0
                        ? nomAlt.MassUomValue
                        : requestItem.Nomenclature.MassUomValue) * factor;
                }

                item.Mass.MassOf1BatchUom = offerMassOf1;
                item.Mass.MassUom = requestItem.Nomenclature.MassUom.Name;

                item.ImportAndDelivery.DeliveryTerms = supplierOffer.DeliveryTerms;
                item.ImportAndDelivery.TotalDeliveryCost = supplierOffer.DeliveryCost;
                
                item.Conversion.CurrencyExchangeRate = 1; //TODO
                item.Conversion.UomRatio = factor;

                item.ResourceConversion.ResourceUom = requestItem.Nomenclature.ResourceUom.Name;
                item.ResourceConversion.ResourceBatchUom = requestItem.Nomenclature.ResourceBatchUom.Name;

                var requestResource = requestItem.Nomenclature.ResourceUomValue > 0
                    ? requestItem.Nomenclature.ResourceUomValue
                    : 1;

                var offerResource = nomAlt?.ResourceUomValue > 0
                    ? nomAlt.ResourceUomValue
                    : (requestItem.Nomenclature.ResourceUomValue > 0
                        ? requestItem.Nomenclature.ResourceUomValue
                        : 1);

                item.ResourceConversion.RequestResource = requestResource;
                item.ResourceConversion.OfferResource = offerResource;
            }

            return result;
        }

        // todo: better algorithm to find match between PR and SO items. Add PRId to so item?
        private SupplierOfferItem FindOfferItem(List<SupplierOfferItem> offerItems, PurchaseRequestItem requestItem)
        {
            var items = offerItems.Where(q => q.NomenclatureId.HasValue && q.NomenclatureId == requestItem.NomenclatureId).ToList();

            if (items.Count == 0) return null;
            if (items.Count == 1)
            {
                return items.First();
            }

            return items.FirstOrDefault(q => q.Position == requestItem.Position);
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

        public void GenerateRawItems(Guid supplierOfferId, bool globalSearch = false)
        {
            var qrySo = _db.SupplierOffers.AsQueryable();
            var qrySoItems = _db.SupplierOfferItems.AsQueryable();
            
            if (globalSearch)
            {
                qrySo = qrySo.IgnoreQueryFilters();
                qrySoItems = qrySoItems.IgnoreQueryFilters();
            }

            qrySoItems.Where(q => q.SupplierOfferId == supplierOfferId).BatchDelete();

            var supplierOffer = qrySo
                .Include(q => q.UploadedDocument)
                .ThenInclude(q => q.Headers)
                .First(q => q.Id == supplierOfferId);

            if (!supplierOffer.SupplierId.HasValue)
            {
                supplierOffer.SupplierId = _supplierService.CreateSupplier(supplierOffer.SupplierName, supplierOffer.OwnerId);
            }

            var table = JsonConvert.DeserializeObject<ExcelTable>(supplierOffer.UploadedDocument.Data);
            var rawItems = new List<SupplierOfferItem>();
            for (var i = 0; i < table.Columns.First(q => q.Type == TableColumnType.Name).Values.Count; i++)
            {
                var rawItem = new SupplierOfferItem
                {
                    SupplierOfferId = supplierOfferId,
                    Position = i + 1, //todo: get from #, № and etc?
                    RawCode = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Code) ? "" : table.GetValue(supplierOffer.UploadedDocument.Headers.Code, i),
                    RawName = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Name) ? "" : table.GetValue(supplierOffer.UploadedDocument.Headers.Name, i),
                    RawQty = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Qty) ? 0 : table.GetDecimalValue(supplierOffer.UploadedDocument.Headers.Qty, i),
                    RawPrice = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Price) ? 0 : table.GetDecimalValue(supplierOffer.UploadedDocument.Headers.Price, i),
                    RawUomStr = string.IsNullOrEmpty(supplierOffer.UploadedDocument.Headers.Uom) ? "" : table.GetValue(supplierOffer.UploadedDocument.Headers.Uom, i)
                };

                rawItem.RawUomId = FindUomId(rawItem.RawUomStr, supplierOffer.OwnerId);
                rawItem.NomenclatureId = FindNomenclatureId(rawItem.RawName, supplierOffer.SupplierId.Value, supplierOffer.OwnerId);                

                rawItems.Add(rawItem);
            }

            var withUndefinedNoms = rawItems.Where(_ => !_.NomenclatureId.HasValue).ToList();
            FixSoNomenclatureIds(withUndefinedNoms);

            foreach (var item in rawItems)
            {
                if (item.NomenclatureId != null && item.RawUomId != null)
                {
                    var rate = _conversionRateService.GetRate(item.RawUomId.Value, item.NomenclatureId.Value, null, supplierOffer.SupplierId).Result;
                    item.CommonFactor = rate.CommonFactor;
                    item.NomenclatureFactor = rate.NomenclatureFactor;
                }
            }

            _db.BulkInsert(rawItems);

            // todo: move to background job?
            _nomenclatureAlternativeService.AddOrUpdateNomenclatureAlts(
                supplierOffer.OwnerId, supplierOffer.SupplierId.Value, ClientType.Supplier,
                rawItems
                    .Where(q => q.NomenclatureId.HasValue)
                    .Select(q => new AddOrUpdateAltDto
                    {
                        NomenclatureId = q.NomenclatureId.Value,
                        Name = q.RawName,
                        Code = q.RawCode,
                        BatchUomId = q.RawUomId
                    }
                    ).ToList());
        }

        private void FixSoNomenclatureIds(IReadOnlyList<SupplierOfferItem> soItems)
        {
            if (soItems.Any(_ => !_.NomenclatureId.HasValue))
            {
                var soId = soItems[0].SupplierOfferId;
                var prItems = (from item in _db.PurchaseRequestItems.IgnoreQueryFilters().Include(_ => _.Nomenclature)
                               where item.NomenclatureId.HasValue &&
                                    item.PurchaseRequest.QuotationRequest.CompetitionList.SupplierOffers.Any(so => so.Id == soId)
                               select item).ToList();

                var unlinkedPrItems = prItems.Where(item => !soItems.Any(soItem => soItem.NomenclatureId == item.NomenclatureId))
                    .Select(item => new
                    {
                        item.Id,
                        item.NomenclatureId,
                        item.RawName,
                        item.RawUomMatchId,
                        item.RawQty
                    });

                var alg = new Levenshtein();

                var algResults = from soItem in soItems.Where(_ => !_.NomenclatureId.HasValue)
                                 let soTerms = _nomenclatureComparisonService.CalculateComparisonTerms(soItem.RawName)
                                 from prItem in unlinkedPrItems
                                 let prTerms = _nomenclatureComparisonService.CalculateComparisonTerms(prItem.RawName)
                                 let isSameUom = soItem.RawUomId == prItem.RawUomMatchId
                                 let distance = _nomenclatureComparisonService.CalculateDistance(soTerms, prTerms, isSameUom, soItem.RawQty, prItem.RawQty)
                                 select new
                                 {
                                     soItem,
                                     prItem,
                                     soDigits = soTerms.AdjustedDigits,
                                     prDigits = prTerms.AdjustedDigits,
                                     nameDistance = distance.NameDistance,
                                     nameWithDims1 = distance.ComparisonName1,
                                     nameWithDims2 = distance.ComparisonName2,
                                     digitsDistance = distance.DigitsDistance,
                                     qtyDiff = distance.QtyDiff,
                                     completeDistance = distance.CompleteDistance,
                                     nameIntersect = distance.NamesIntersect,
                                     longestNameSubstr = distance.NamesLongestSubstringLen,
                                     soItemDims = soTerms.NomDimensions,
                                     prItemDims = prTerms.NomDimensions
                                 };
                
                while (algResults.Any())
                {
                    algResults = algResults.OrderBy(el => el.completeDistance);
                    var bestMatch = algResults.First();
                    bestMatch.soItem.NomenclatureId = bestMatch.prItem.NomenclatureId;
                    algResults = algResults.Where(el => el.soItem.Id != bestMatch.soItem.Id && el.prItem.Id != bestMatch.prItem.Id);
                }
            }
        }

        private static int LongestCommonSubstring(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return 0;

            var num = new int[str1.Length, str2.Length];
            int maxlen = 0;

            for (int i = 0; i < str1.Length; i++)
            {
                for (int j = 0; j < str2.Length; j++)
                {
                    if (str1[i] != str2[j])
                        num[i, j] = 0;
                    else
                    {
                        if ((i == 0) || (j == 0))
                            num[i, j] = 1;
                        else
                            num[i, j] = 1 + num[i - 1, j - 1];

                        if (num[i, j] > maxlen)
                        {
                            maxlen = num[i, j];
                        }
                    }
                }
            }
            return maxlen;
        }

        private Guid? FindUomId(string uomName, Guid ownerId)
        {
            var res = _uomService.Autocomplete(uomName, ownerId);
            var fullMatch = res.Items?.FirstOrDefault(q => q.IsFullMatch);
            return fullMatch?.Id;
        }

        private Guid? FindNomenclatureId(string nomName, Guid clientId, Guid ownerId)
        {
            var nomRes = _nomenclatureService.Autocomplete(new AutocompleteOptions
            {
                Query = nomName,
                ClientId = clientId,
                ClientType = ClientType.Supplier,
                SearchInAlts = true,
                OwnerId = ownerId
            });

            if (nomRes.Items.Count(q => q.IsFullMatch) == 1)
            {
                return nomRes.Items.First(q => q.IsFullMatch).Id;
            }

            return null;
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
            var res = new SOMatchItemsVm
            {
                Items = entities.Adapt<List<SOMatchItemsVm.Item>>(),
                SupplierName = supplierOffer.SupplierName,
                SupplierId = supplierOffer.SupplierId
            };

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

            using (var transaction = _db.Database.BeginTransaction())
            {
                if (so.UploadedDocumentId.HasValue)
                {
                    _uploadedDocumentService.Delete(so.UploadedDocumentId.Value);
                }

                _db.PriceReductionEmails.RemoveRange(_db.PriceReductionEmails.Where(q => q.SupplierOfferId == id));
                _db.SupplierOfferItems.RemoveRange(_db.SupplierOfferItems.Where(q => q.SupplierOfferId == id));
                _db.SupplierOffers.Remove(so);
                _db.SaveChanges();
                transaction.Commit();
            }

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
            => config.NewConfig<SupplierOfferItem, SOMatchItemsVm.Item>()
                .Map(d => d.NomenclatureUom, s => s.NomenclatureId.HasValue ? s.Nomenclature.BatchUom.Name : null)
                .Map(q => q.RawUomStr, q => q.RawUomId.HasValue ? q.RawUom.Name : q.RawUomStr);
    }
}
