using System;
using System.Linq;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
using DigitalPurchasing.Models.Counters;
using DigitalPurchasing.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Guid CompanyId() => _httpContextAccessor?.HttpContext?.User.CompanyId() ?? Guid.Empty;

        public DbSet<Root> Roots { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<NomenclatureAlternativeLink> NomenclatureAlternativeLinks { get; set; }
        public DbSet<NomenclatureAlternative> NomenclatureAlternatives { get; set; }
        public DbSet<NomenclatureCategory> NomenclatureCategories { get; set; }
        public DbSet<UnitsOfMeasurement> UnitsOfMeasurements { get; set; }
        public DbSet<UomConversionRate> UomConversionRates { get; set; }

        public DbSet<QuotationRequest> QuotationRequests { get; set; }
        public DbSet<QuotationRequestEmail> QuotationRequestEmails { get; set; }

        public DbSet<CompetitionList> CompetitionLists { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierContactPerson> SupplierContactPersons { get; set; }
        public DbSet<SupplierOffer> SupplierOffers { get; set; }
        public DbSet<SupplierOfferItem> SupplierOfferItems { get; set; }

        public DbSet<AnalysisVariant> AnalysisVariants { get; set; }
        public DbSet<AnalysisResultItem> AnalysisResultItems { get; set; }

        public DbSet<SupplierCategory> SupplierCategories { get; set; }

        #region Counters

        public DbSet<PRCounter> PRCounters { get; set; }
        public DbSet<QRCounter> QRCounters { get; set; }
        public DbSet<CLCounter> CLCounters { get; set; }
        public DbSet<SOCounter> SOCounters { get; set; }
        public DbSet<CustomerCounter> CustomerCounters { get; set; }
        public DbSet<SupplierCounter> SupplierCounters { get; set; }

        #endregion

        public DbSet<ColumnName> ColumnNames { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<UploadedDocument> UploadedDocuments { get; set; }
        public DbSet<UploadedDocumentHeaders> UploadedDocumentHeaders { get; set; }

        public DbSet<ReceivedEmail> ReceivedEmails { get; set; }

        public DbSet<SelectedSupplier> SelectedSuppliers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.ToTable("UserRoles");
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>(e =>
            {
                e.ToTable("Roles");
                e.HasData(new Role
                {
                    Id = new Guid("523a1a9dfa7045ed9ca01ddf3f037779"),
                    ConcurrencyStamp = "523a1a9dfa7045ed9ca01ddf3f037779",
                    Name = Consts.Roles.CompanyOwner,
                    NormalizedName = Consts.Roles.CompanyOwner.ToUpper()
                });
                e.HasData(new Role
                {
                    Id = new Guid("8de94260ade74062aec1a1568f62ecd1"),
                    ConcurrencyStamp = "8de94260ade74062aec1a1568f62ecd1",
                    Name = Consts.Roles.Admin,
                    NormalizedName = Consts.Roles.Admin.ToUpper()
                });
            });
            builder.Entity<UserClaim>().ToTable("UserClaims");
            builder.Entity<UserLogin>().ToTable("UserLogins");
            builder.Entity<UserToken>().ToTable("UserTokens");
            builder.Entity<RoleClaim>().ToTable("RoleClaims");

            builder.Entity<NomenclatureCategory>().HasOne(q => q.Parent).WithMany(q => q.Children).HasForeignKey(q => q.ParentId);
            builder.Entity<NomenclatureCategory>().HasOne(q => q.Owner).WithMany().HasForeignKey(q => q.OwnerId);

            builder.Entity<Nomenclature>().HasOne(q => q.Category).WithMany(q => q.Nomenclatures).HasForeignKey(q => q.CategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.BatchUom).WithMany(q => q.BatchNomenclatures).HasForeignKey(q => q.BatchUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.MassUom).WithMany(q => q.MassNomenclatures).HasForeignKey(q => q.MassUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.ResourceUom).WithMany(q => q.ResourceNomenclatures).HasForeignKey(q => q.ResourceUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.ResourceBatchUom).WithMany(q => q.ResourceBatchNomenclatures).HasForeignKey(q => q.ResourceBatchUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.PackUom).WithMany().HasForeignKey(q => q.PackUomId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NomenclatureAlternative>().HasOne(q => q.Nomenclature).WithMany().HasForeignKey(q => q.NomenclatureId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.BatchUom).WithMany(q => q.BatchNomenclatureAlternatives).HasForeignKey(q => q.BatchUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.MassUom).WithMany(q => q.MassNomenclatureAlternatives).HasForeignKey(q => q.MassUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.ResourceUom).WithMany(q => q.ResourceNomenclatureAlternatives).HasForeignKey(q => q.ResourceUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.ResourceBatchUom).WithMany(q => q.ResourceBatchNomenclatureAlternatives).HasForeignKey(q => q.ResourceBatchUomId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupplierCategory>()
                .HasOne(q => q.NomenclatureCategory).WithMany().HasForeignKey(q => q.NomenclatureCategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<SupplierCategory>()
                .HasOne(q => q.PrimaryContactPerson).WithMany().HasForeignKey(q => q.PrimaryContactPersonId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<SupplierCategory>()
                .HasOne(q => q.SecondaryContactPerson).WithMany().HasForeignKey(q => q.SecondaryContactPersonId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Customer>(e =>
            {
                e.HasMany(q => q.Requests).WithOne(q => q.Customer).HasForeignKey(q => q.CustomerId).OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(q => new { q.OwnerId, q.PublicId }).IsUnique();
            });

            builder.Entity<PurchaseRequest>(e =>
            {
                e.HasMany(q => q.Items).WithOne(q => q.PurchaseRequest).HasForeignKey(q => q.PurchaseRequestId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.UploadedDocument).WithOne().HasForeignKey<PurchaseRequest>(q => q.UploadedDocumentId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<PurchaseRequestItem>().HasOne(q => q.Nomenclature).WithMany().HasForeignKey(q => q.NomenclatureId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PurchaseRequestItem>().HasOne(q => q.RawUomMatch).WithMany(q => q.PurchasingRequestItems).HasForeignKey(q => q.RawUomMatchId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuotationRequest>(e =>
            {
                e.HasOne(q => q.PurchaseRequest).WithOne().HasForeignKey<QuotationRequest>(q => q.PurchaseRequestId).OnDelete(DeleteBehavior.Restrict);
                e.HasMany(q => q.Emails).WithOne(q => q.Request).HasForeignKey(q => q.RequestId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CompetitionList>().HasOne(q => q.QuotationRequest).WithOne().HasForeignKey<CompetitionList>(q => q.QuotationRequestId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Supplier>(e =>
            {
                e.HasMany(q => q.Offers).WithOne(q => q.Supplier).HasForeignKey(q => q.SupplierId).OnDelete(DeleteBehavior.Restrict);
                e.HasMany(q => q.ContactPersons).WithOne(q => q.Supplier).HasForeignKey(q => q.SupplierId).OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(q => new { q.OwnerId, q.Inn }).IsUnique().HasFilter($"{nameof(Supplier.Name)} IS NOT NULL AND {nameof(Supplier.Inn)} IS NOT NULL");
                e.HasIndex(q => new { q.OwnerId, q.PublicId }).IsUnique();
            });

            builder.Entity<SupplierOffer>(e =>
            {
                e.HasOne(q => q.Owner).WithMany().HasForeignKey(q => q.OwnerId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.UploadedDocument).WithOne().HasForeignKey<SupplierOffer>(q => q.UploadedDocumentId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<SelectedSupplier>(e =>
            {
                e.HasOne(q => q.Owner).WithMany().HasForeignKey(q => q.OwnerId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.Nomenclature).WithMany().HasForeignKey(q => q.NomenclatureId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.Supplier).WithMany().HasForeignKey(q => q.SupplierId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.Root).WithMany(q => q.SelectedSuppliers).HasForeignKey(q => q.RootId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<UomConversionRate>().HasOne(q => q.FromUom).WithMany(q => q.FromConversionRates).HasForeignKey(q => q.FromUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UomConversionRate>().HasOne(q => q.ToUom).WithMany(q => q.ToConversionRates).HasForeignKey(q => q.ToUomId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Delivery>().HasMany(q => q.PurchaseRequests).WithOne(q => q.Delivery).HasForeignKey(q => q.DeliveryId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Delivery>().HasMany(q => q.QuotationRequests).WithOne(q => q.Delivery).HasForeignKey(q => q.DeliveryId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AnalysisVariant>(e =>
            {
                e.HasOne(q => q.CompetitionList).WithMany().HasForeignKey(q => q.CompetitionListId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(q => q.Owner).WithMany().HasForeignKey(q => q.OwnerId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<AnalysisResultItem>(e =>
            {
                e.HasOne(q => q.Nomenclature).WithMany().HasForeignKey(q => q.NomenclatureId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.Variant).WithMany().HasForeignKey(q => q.VariantId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.Supplier).WithMany().HasForeignKey(q => q.SupplierId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CustomerCounter>().HasIndex(q => q.OwnerId).IsUnique();
            builder.Entity<SupplierCounter>().HasIndex(q => q.OwnerId).IsUnique();

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
            builder.Entity<Customer>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<PurchaseRequest>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<QuotationRequest>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<CompetitionList>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<Supplier>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<SupplierOffer>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<SupplierContactPerson>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<Delivery>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<Nomenclature>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<NomenclatureAlternative>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<NomenclatureCategory>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<ColumnName>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<UnitsOfMeasurement>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<UomConversionRate>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<UploadedDocument>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<AnalysisVariant>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<AnalysisResultItem>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<SelectedSupplier>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<PRCounter>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<QRCounter>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<CLCounter>().HasQueryFilter(o => o.OwnerId == CompanyId());
            builder.Entity<SOCounter>().HasQueryFilter(o => o.OwnerId == CompanyId());
        }

        public override int SaveChanges()
        {
            var addedEntities = ChangeTracker.Entries().Where(c => c.State == EntityState.Added).Select(q => q.Entity).ToList();
            
            foreach (var entity in addedEntities.OfType<IHaveOwner>())
            {
                if (entity.OwnerId == Guid.Empty)
                {
                    entity.OwnerId = CompanyId();
                }
            }

            return base.SaveChanges();
        }
    }
}

