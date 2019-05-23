using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models.SSR;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalPurchasing.Services
{
    public class SelectedSupplierService : ISelectedSupplierService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAnalysisService _analysisService;
        private readonly ICompetitionListService _competitionListService;
        private readonly ISupplierOfferService _supplierOfferService;
        private readonly ILogger _logger;

        public SelectedSupplierService(
            ApplicationDbContext db,
            IAnalysisService analysisService,
            ICompetitionListService competitionListService,
            ISupplierOfferService supplierOfferService,
            ILogger<SelectedSupplierService> logger)
        {
            _db = db;
            _analysisService = analysisService;
            _competitionListService = competitionListService;
            _supplierOfferService = supplierOfferService;
            _logger = logger;
        }

        public async Task<GenerateReportDataResult> GenerateReportData(Guid ownerId, Guid userId, Guid variantId)
        {
            var selectedVariant = await _db.AnalysisVariants
                .IgnoreQueryFilters()
                .FirstAsync(q => q.OwnerId == ownerId && q.Id == variantId);

            if (!selectedVariant.CompetitionListId.HasValue)
            {
                throw new ArgumentException(nameof(variantId));
            }

            var cl = _competitionListService.GetById(selectedVariant.CompetitionListId.Value);

            var root = await _db.Roots.FirstAsync(q => q.CompetitionListId == cl.Id);
            
            var variants = await _db.AnalysisVariants
                .Where(q => q.CompetitionListId == selectedVariant.CompetitionListId)
                .OrderBy(q => q.CreatedOn)
                .ToListAsync();

            var data = _analysisService.GetData(selectedVariant.CompetitionListId.Value);

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // report
                    var ssReportEntry = await _db.SSReports.AddAsync(new SSReport
                    {
                        OwnerId = ownerId,
                        UserId = userId,
                        RootId = root.Id,
                        CLCreatedOn = cl.CreatedOn,
                        CLNumber = cl.PublicId,
                        SelectedVariantNumber = variants.IndexOf(variants.Find(q => q.IsSelected)) + 1
                    });

                    await _db.SaveChangesAsync();
                    var ssReport = ssReportEntry.Entity;

                    // customer
                    var ssCustomerEntry = await _db.SSCustomers.AddAsync(new SSCustomer
                    {
                        Name = data.CustomerRequest.Name,
                        InternalId = data.CustomerRequest.CustomerId,
                        ReportId = ssReport.Id,
                        PRNumber = cl.PurchaseRequest.PublicId,
                        PRCreatedOn = cl.PurchaseRequest.CreatedOn
                    });
                    await _db.SaveChangesAsync();
                    var ssCustomer = ssCustomerEntry.Entity;

                    // customer items
                    var ssCustomerItems = cl.PurchaseRequest.Items.Select(q => new SSCustomerItem
                    {
                        CustomerId = ssCustomer.Id,
                        Code = q.Nomenclature.Code,
                        Uom = q.Nomenclature.BatchUomName,
                        Name = q.RawName,
                        Quantity = q.RawQty,
                        Position = q.Position,
                        InternalId = q.Id,
                        NomenclatureId = q.NomenclatureId
                    }).ToList();

                    await _db.SSCustomerItems.AddRangeAsync(ssCustomerItems);

                    var ssSuppliers = new List<SSSupplier>();

                    // supplier items
                    foreach (var supplierOffer in cl.SupplierOffers)
                    {
                        // supplier + so data
                        var ssSupplier = new SSSupplier
                        {
                            Name = supplierOffer.Supplier.Name,
                            InternalId = supplierOffer.Supplier.Id,
                            SOCreatedOn = supplierOffer.CreatedOn,
                            SONumber = supplierOffer.PublicId,
                            SOInternalId = supplierOffer.Id
                        };

                        await _db.SSSuppliers.AddAsync(ssSupplier);
                        await _db.SaveChangesAsync();

                        ssSuppliers.Add(ssSupplier);

                        var soDetails = _supplierOfferService.GetDetailsById(supplierOffer.Id);

                        var ssSupplierItems = supplierOffer.Items.Select(q =>
                        {
                            var detailsItem = soDetails.Items.Find(i => i.Offer.ItemId == q.Id);
                            return new SSSupplierItem
                            {
                                SupplierId = ssSupplier.Id,
                                Name = q.RawName,
                                Quantity = q.RawQty,
                                Price = q.RawPrice,
                                NomenclatureId = q.NomenclatureId,
                                InternalId = q.Id,
                                ConvertedQuantity = detailsItem.Conversion.OfferQty,
                                ConvertedPrice = detailsItem.Conversion.OfferPrice
                            };
                        }).ToList();
                        await _db.SSSupplierItems.AddRangeAsync(ssSupplierItems);
                        await _db.SaveChangesAsync();
                    }

                    // variants
                    foreach (var variantData in data.Variants)
                    {
                        // variant
                        var variant = variants.Find(e => e.Id == variantData.Id);
                        var ssVariant = new SSVariant
                        {
                            ReportId = ssReport.Id,
                            IsSelected = variant.IsSelected,
                            Number = variants.IndexOf(variant) + 1,
                            InternalId = variant.Id,
                            CreatedOn = variant.CreatedOn
                        };
                        var ssVariantEntry = await _db.SSVariants.AddAsync(ssVariant);
                        await _db.SaveChangesAsync();
                        ssVariant = ssVariantEntry.Entity;

                        var datas = new List<SSData>();

                        // variant datas
                        foreach (var resultByItem in variantData.ResultsByItem)
                        {
                            var ssSupplier = ssSuppliers.Find(q => q.SOInternalId == resultByItem.SupplierOfferId);

                            // variant data
                            var ssData = new SSData
                            {
                                VariantId = ssVariant.Id,
                                SupplierId = ssSupplier.Id,
                                NomenclatureId = resultByItem.ItemId,
                                Quantity = resultByItem.Quantity,
                                Price = resultByItem.Price
                            };

                            await _db.SSDatas.AddAsync(ssData);
                            await _db.SaveChangesAsync();

                            datas.Add(ssData);
                        }

                        if (variant.IsSelected)
                        {
                            ssReport.SelectedVariantTotalPrice = datas.Sum(q => q.Quantity * q.Price);
                            await _db.SaveChangesAsync();
                        }
                    }

                    // done
                    transaction.Commit();

                    return new GenerateReportDataResult { IsSuccess = true, ReportId = ssReport.Id };
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unable to generate report");
                    return new GenerateReportDataResult { IsSuccess = false, ReportId = Guid.Empty };
                }
            }
        }

        public async Task<IEnumerable<SSReportSimple>> GetReports(Guid clId)
        {
            var root = await _db.Roots.FirstAsync(q => q.CompetitionListId == clId);
            var reports = await _db.SSReports
                .Include(q => q.User)
                .Where(q => q.RootId == root.Id)
                .OrderByDescending(q => q.CreatedOn)
                .ToListAsync();

            var result = reports.Select(r => new SSReportSimple
            {
                ReportId = r.Id,
                CreatedOn = r.CreatedOn,
                UserFirstName = r.User.FirstName,
                UserLastName = r.User.LastName,
                Currency = "RUB",
                SelectedVariantNumber = r.SelectedVariantNumber,
                SelectedVariantTotalPrice = r.SelectedVariantTotalPrice
            }).ToList();

            return result;
        }

        public async Task<SSReportDto> GetReport(Guid reportId)
        {
            var report = await _db.SSReports.Include(q => q.User).FirstAsync(q => q.Id == reportId);
           
            var customer = await _db.SSCustomers.FirstAsync(q => q.ReportId == reportId);
            var customerItems = await _db.SSCustomerItems.Where(q => q.CustomerId == customer.Id).ToListAsync();
            
            var suppliersIds = await _db.SSDatas
                .Include(q => q.Variant)
                .Where(q => q.Variant.ReportId == reportId)
                .Select(q => q.SupplierId)
                .Distinct()
                .ToListAsync();

            var suppliers = await _db.SSSuppliers.Where(q => suppliersIds.Contains(q.Id)).OrderBy(q => q.SOCreatedOn).ToListAsync();
            var supplierItems = await _db.SSSupplierItems.Where(q => suppliersIds.Contains(q.SupplierId)).ToListAsync();
            
            var datas = await _db.SSDatas
                .Include(q => q.Variant)
                .Include(q => q.Supplier)
                .Where(q => q.Variant.ReportId == reportId)
                .ToListAsync();

            var variants = await _db.SSVariants.Where(q => q.ReportId == reportId).ToListAsync();

            var result = report.Adapt<SSReportDto>();
            result.Customer = customer.Adapt<SSCustomerDto>();
            result.CustomerItems = customerItems.Adapt<List<SSCustomerItemDto>>();
            result.Suppliers = suppliers.Adapt<List<SSSupplierDto>>();
            result.SSSupplierItems = supplierItems.Adapt<List<SSSupplierItemDto>>();
            result.Datas = datas.Adapt<List<SSDataDto>>();
            result.Variants = variants.Adapt<List<SSVariantDto>>();

            return result;
        }
    }
}
