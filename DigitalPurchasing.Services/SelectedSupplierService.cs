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
        private readonly ILogger _logger;

        public SelectedSupplierService(
            ApplicationDbContext db,
            IAnalysisService analysisService,
            ICompetitionListService competitionListService,
            ILogger<SelectedSupplierService> logger)
        {
            _db = db;
            _analysisService = analysisService;
            _competitionListService = competitionListService;
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
                    var ssReportEntry = await _db.SSReports.AddAsync(new SSReport { OwnerId = ownerId, UserId = userId, RootId = root.Id });
                    await _db.SaveChangesAsync();
                    var ssReport = ssReportEntry.Entity;

                    // customer
                    var ssCustomerEntry = await _db.SSCustomers.AddAsync(new SSCustomer
                    {
                        Name = data.CustomerRequest.Name,
                        InternalId = data.CustomerRequest.CustomerId,
                        ReportId = ssReport.Id
                    });
                    await _db.SaveChangesAsync();
                    var ssCustomer = ssCustomerEntry.Entity;

                    // customer items
                    var ssCustomerItems = cl.PurchaseRequest.Items.Select(q => new SSCustomerItem
                    {
                        CustomerId = ssCustomer.Id,
                        Name = ssCustomer.Name,
                        Quantity = q.RawQty,
                        Position = q.Position,
                        InternalId = q.Id,
                        NomenclatureId = q.NomenclatureId,
                    }).ToList();

                    await _db.SSCustomerItems.AddRangeAsync(ssCustomerItems);

                    // suppliers
                    var ssSuppliers = cl.SupplierOffers.Distinct()
                        .Select(q => new SSSupplier { Name = q.Supplier.Name, InternalId = q.Supplier.Id })
                        .ToList();
                    await _db.SSSuppliers.AddRangeAsync(ssSuppliers);
                    await _db.SaveChangesAsync();

                    // supplier items
                    foreach (var supplierOffer in cl.SupplierOffers)
                    {
                        var ssSupplierItems = supplierOffer.Items.Select(q => new SSSupplierItem
                        {
                            SupplierId = ssSuppliers.Find(e => e.InternalId == supplierOffer.Supplier.Id).Id,
                            Name = q.RawName,
                            Quantity = q.RawQty,
                            Price = q.RawPrice,
                            NomenclatureId = q.NomenclatureId,
                            InternalId = q.Id,
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
                            Number = variants.FindIndex(e => e.Id == variantData.Id) + 1,
                            InternalId = variant.Id
                        };
                        var ssVariantEntry = await _db.SSVariants.AddAsync(ssVariant);
                        await _db.SaveChangesAsync();
                        ssVariant = ssVariantEntry.Entity;

                        // variant datas
                        foreach (var resultByItem in variantData.ResultsByItem)
                        {
                            var dbSupplierId = data.SupplierOffers.Find(q => q.Id == resultByItem.SupplierId).SupplierId;
                            
                            var ssSupplier = ssSuppliers.Find(q => q.InternalId == dbSupplierId);

                            // variant data
                            var ssData = new SSData
                            {
                                VariantId = ssVariant.Id,
                                SupplierId = ssSupplier.Id,
                                NomenclatureId = resultByItem.ItemId,
                                Quantity = resultByItem.Quantity,
                            };

                            await _db.SSDatas.AddRangeAsync(ssData);
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
            
            var result = reports.Select(q => new SSReportSimple
            {
                ReportId = q.Id,
                CreatedOn = q.CreatedOn,
                UserFirstName = q.User.FirstName,
                UserLastName = q.User.LastName
            }).ToList();

            return result;
        }

        public async Task<SSReportDto> GetReport(Guid reportId)
        {
            var report = await _db.SSReports.Include(q => q.User).FirstAsync();
            var result = report.Adapt<SSReportDto>();

            var variants = await _db.SSVariants.Where(q => q.ReportId == reportId).ToListAsync();

            var datas = await _db.SSDatas
                .Include(q => q.Variant)
                .Include(q => q.Supplier)
                .Where(q => q.Variant.ReportId == reportId)
                .ToListAsync();

            var customer = await _db.SSCustomers.FirstAsync(q => q.ReportId == reportId);

            return result;
        }
    }
}
