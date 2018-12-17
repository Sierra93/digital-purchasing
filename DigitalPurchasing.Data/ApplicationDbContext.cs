using System;
using System.Linq;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
using DigitalPurchasing.Models.Counters;
using DigitalPurchasing.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        private readonly ITenantService _tenantService;

        private Guid CompanyId => _tenantService.Get().CompanyId;

        public DbSet<Company> Companies { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<NomenclatureAlternative> NomenclatureAlternatives { get; set; }
        public DbSet<NomenclatureCategory> NomenclatureCategories { get; set; }
        public DbSet<UnitsOfMeasurement> UnitsOfMeasurements { get; set; }
        public DbSet<UomConversionRate> UomConversionRates { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public DbSet<QuotationRequest> QuotationRequests { get; set; }
        public DbSet<CompetitionList> CompetitionLists { get; set; }
        public DbSet<SupplierOffer> SupplierOffers { get; set; }
        public DbSet<SupplierOfferItem> SupplierOfferItems { get; set; }

        #region
        public DbSet<AnalysisVariant> AnalysisVariants { get; set; }
        #endregion

        #region Counters

        public DbSet<PRCounter> PRCounters { get; set; }
        public DbSet<QRCounter> QRCounters { get; set; }
        public DbSet<CLCounter> CLCounters { get; set; }
        public DbSet<SOCounter> SOCounters { get; set; }

        #endregion

        public DbSet<ColumnName> ColumnNames { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<UploadedDocument> UploadedDocuments { get; set; }
        public DbSet<UploadedDocumentHeaders> UploadedDocumentHeaders { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService) : base(options) => _tenantService = tenantService;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<UserClaim>().ToTable("UserClaims");
            builder.Entity<UserLogin>().ToTable("UserLogins");
            builder.Entity<UserRole>().ToTable("UserRoles");
            builder.Entity<UserToken>().ToTable("UserTokens");
            builder.Entity<RoleClaim>().ToTable("RoleClaims");

            builder.Entity<NomenclatureCategory>().HasOne(q => q.Parent).WithMany(q => q.Children).HasForeignKey(q => q.ParentId);
            builder.Entity<NomenclatureCategory>().HasOne(q => q.Owner).WithMany(q => q.NomenclatureCategories).HasForeignKey(q => q.OwnerId);

            builder.Entity<Nomenclature>().HasOne(q => q.Category).WithMany(q => q.Nomenclatures).HasForeignKey(q => q.CategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.BatchUom).WithMany(q => q.BatchNomenclatures).HasForeignKey(q => q.BatchUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.MassUom).WithMany(q => q.MassNomenclatures).HasForeignKey(q => q.MassUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.ResourceUom).WithMany(q => q.ResourceNomenclatures).HasForeignKey(q => q.ResourceUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.ResourceBatchUom).WithMany(q => q.ResourceBatchNomenclatures).HasForeignKey(q => q.ResourceBatchUomId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NomenclatureAlternative>().HasOne(q => q.Nomenclature).WithMany(q => q.Alternatives).HasForeignKey(q => q.NomenclatureId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.BatchUom).WithMany(q => q.BatchNomenclatureAlternatives).HasForeignKey(q => q.BatchUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.MassUom).WithMany(q => q.MassNomenclatureAlternatives).HasForeignKey(q => q.MassUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.ResourceUom).WithMany(q => q.ResourceNomenclatureAlternatives).HasForeignKey(q => q.ResourceUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.ResourceBatchUom).WithMany(q => q.ResourceBatchNomenclatureAlternatives).HasForeignKey(q => q.ResourceBatchUomId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseRequest>(e =>
            {
                e.HasMany(q => q.Items).WithOne(q => q.PurchaseRequest).HasForeignKey(q => q.PurchaseRequestId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.UploadedDocument).WithOne().HasForeignKey<PurchaseRequest>(q => q.UploadedDocumentId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<SupplierOffer>(e =>
            {
                e.HasOne(q => q.Owner).WithMany().HasForeignKey(q => q.OwnerId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.UploadedDocument).WithOne().HasForeignKey<SupplierOffer>(q => q.UploadedDocumentId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<PurchaseRequestItem>().HasOne(q => q.Nomenclature).WithMany(q => q.PurchasingRequestItems).HasForeignKey(q => q.NomenclatureId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PurchaseRequestItem>().HasOne(q => q.RawUomMatch).WithMany(q => q.PurchasingRequestItems).HasForeignKey(q => q.RawUomMatchId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuotationRequest>().HasOne(q => q.PurchaseRequest).WithOne().HasForeignKey<QuotationRequest>(q => q.PurchaseRequestId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CompetitionList>().HasOne(q => q.QuotationRequest).WithOne().HasForeignKey<CompetitionList>(q => q.QuotationRequestId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UomConversionRate>().HasOne(q => q.FromUom).WithMany(q => q.FromConversionRates).HasForeignKey(q => q.FromUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UomConversionRate>().HasOne(q => q.ToUom).WithMany(q => q.ToConversionRates).HasForeignKey(q => q.ToUomId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Delivery>().HasMany(q => q.PurchaseRequests).WithOne(q => q.Delivery).HasForeignKey(q => q.DeliveryId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Delivery>().HasMany(q => q.QuotationRequests).WithOne(q => q.Delivery).HasForeignKey(q => q.DeliveryId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AnalysisVariant>(e =>
            {
                e.HasOne(q => q.CompetitionList).WithMany().HasForeignKey(q => q.CompetitionListId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(q => q.Owner).WithMany().HasForeignKey(q => q.OwnerId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Currency>(e =>
            {
                e.HasData(new Currency
                {
                    Id = new Guid("77fd8325b90c42ae9e03a2f92ea6688e"),
                    Name = "RUB",
                    CreatedOn = new DateTime(2018, 11, 5)
                });
                e.HasData(new Currency
                {
                    Id = new Guid("bb4ebd3eba244c8688c940306a171349"),
                    Name = "USD",
                    CreatedOn = new DateTime(2018, 11, 5)
                });
                e.HasData(new Currency
                {
                    Id = new Guid("00a0127ef9dc4b1ab4db13c014e827ec"),
                    Name = "EUR",
                    CreatedOn = new DateTime(2018, 11, 5)
                });
            });

            // default filters to show company data
            builder.Entity<PurchaseRequest>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<QuotationRequest>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<CompetitionList>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<SupplierOffer>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<Delivery>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<Nomenclature>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<NomenclatureAlternative>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<NomenclatureCategory>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<ColumnName>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<UnitsOfMeasurement>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<UomConversionRate>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<UploadedDocument>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<AnalysisVariant>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<PRCounter>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<QRCounter>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<CLCounter>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<SOCounter>().HasQueryFilter(o => o.OwnerId == CompanyId);
        }

        public override int SaveChanges()
        {
            var companyId = _tenantService.Get().CompanyId;

            var addedEntities = ChangeTracker.Entries().Where(c => c.State == EntityState.Added).Select(q => q.Entity).ToList();

            foreach (var entity in addedEntities.OfType<IHaveOwner>())
            {
                entity.OwnerId = companyId;
            }

            return base.SaveChanges();
        }
    }
}

