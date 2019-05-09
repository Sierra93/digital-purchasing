using System;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models.SSR;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class SelectedSupplierService : ISelectedSupplierService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAnalysisService _analysisService;
        private readonly ICompetitionListService _competitionListService;

        public SelectedSupplierService(
            ApplicationDbContext db,
            IAnalysisService analysisService,
            ICompetitionListService competitionListService)
        {
            _db = db;
            _analysisService = analysisService;
            _competitionListService = competitionListService;
        }

        public async Task GenerateReport(Guid ownerId, Guid variantId)
        {
            var selectedVariant = await _db.AnalysisVariants
                .IgnoreQueryFilters()
                .FirstAsync(q => q.OwnerId == ownerId && q.Id == variantId);

            if (!selectedVariant.CompetitionListId.HasValue)
            {
                throw new ArgumentException(nameof(variantId));
            }

            var cl = _competitionListService.GetById(selectedVariant.CompetitionListId.Value);
            
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
                    var ssReportEntry = await _db.SSReports.AddAsync(new SSReport { OwnerId = ownerId });
                    await _db.SaveChangesAsync();
                    var ssReport = ssReportEntry.Entity;

                    // customer
                    var ssCustomerEntry = await _db.SSCustomers.AddAsync(new SSCustomer { Name = data.Customer.Name, InternalId = data.Customer.Id });
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
                    var ssSuppliers = cl.SupplierOffers
                        .Select(q => new SSSupplier { Name = q.Supplier.Name, InternalId = q.Supplier.Id })
                        .ToList();
                    await _db.SSSuppliers.AddRangeAsync(ssSuppliers);
                    await _db.SaveChangesAsync();

                    // supplier items
                    foreach (var supplierOffer in cl.SupplierOffers)
                    {
                        var ssSupplierItems = supplierOffer.Items.Select(q => new SSSupplierItem
                        {
                            SupplierId = ssSuppliers.Find(e => e.InternalId == supplierOffer.Id).Id,
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
                            // variant data
                            var ssSupplier = ssSuppliers.Find(q => q.InternalId == resultByItem.SupplierId);
                            
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
                }
                catch (Exception)
                {
                    // todo
                }
            }
        }
    }
}
