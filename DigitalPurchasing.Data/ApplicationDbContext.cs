using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
using DigitalPurchasing.Models.Counters;
using DigitalPurchasing.Models.Identity;
using DigitalPurchasing.Models.SSR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DigitalPurchasing.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Guid CompanyId() => _httpContextAccessor?.HttpContext?.User.CompanyId() ?? Guid.Empty;

        [DbFunction("udf_LevenshteinDistance")]
        public static int? LevenshteinDistanceFunc(string str1, string str2, int maxDistance) =>
            throw new Exception("udf_LevenshteinDistance scalar function should be in the database");

        [DbFunction("udf_LongestCommonSubstringLen")]
        public static int LongestCommonSubstringLenFunc(string str1, string str2) =>
            throw new Exception("udf_LongestCommonSubstringLen scalar function should be in the database");

        public DbSet<Root> Roots { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<NomenclatureComparisonData> NomenclatureComparisonDatas { get; set; }
        public DbSet<NomenclatureAlternativeLink> NomenclatureAlternativeLinks { get; set; }
        public DbSet<NomenclatureAlternative> NomenclatureAlternatives { get; set; }
        public DbSet<NomenclatureCategory> NomenclatureCategories { get; set; }
        public DbSet<UnitsOfMeasurement> UnitsOfMeasurements { get; set; }
        public DbSet<UomConversionRate> UomConversionRates { get; set; }
        public DbSet<DefaultUom> DefaultUoms { get; set; }

        public DbSet<QuotationRequest> QuotationRequests { get; set; }
        public DbSet<QuotationRequestEmail> QuotationRequestEmails { get; set; }

        public DbSet<CompetitionList> CompetitionLists { get; set; }
        public DbSet<PriceReductionEmail> PriceReductionEmails { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public DbSet<PurchaseRequestAttachment> PurchaseRequestAttachments { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierContactPerson> SupplierContactPersons { get; set; }
        public DbSet<SupplierOffer> SupplierOffers { get; set; }
        public DbSet<SupplierOfferItem> SupplierOfferItems { get; set; }

        public DbSet<AnalysisVariant> AnalysisVariants { get; set; }

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
        public DbSet<File> Files { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }
        public DbSet<TermsFile> TermsFiles { get; set; }

        #region Selected supplier report

        public DbSet<SSReport> SSReports { get; set; }
        public DbSet<SSVariant> SSVariants { get; set; }
        public DbSet<SSData> SSDatas { get; set; }
        public DbSet<SSCustomer> SSCustomers { get; set; }
        public DbSet<SSSupplier> SSSuppliers { get; set; }
        public DbSet<SSCustomerItem> SSCustomerItems { get; set; }
        public DbSet<SSSupplierItem> SSSupplierItems { get; set; }

        #endregion

        public DbSet<AppNGram> AppNGrams { get; set; }
        public DbSet<NomenclatureComparisonDataNGram> NomenclatureComparisonDataNGrams { get; set; }

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

            builder.Entity<User>(e =>
            {
                e.Property(q => q.PRDiscountPercentage)
                    .HasDefaultValue(Consts.Settings.PRDiscountPercentage);

                e.Property(q => q.QuotationRequestResponseHours)
                    .HasDefaultValue(Consts.Settings.QuotationRequestResponseHours);

                e.Property(q => q.PriceReductionResponseHours)
                    .HasDefaultValue(Consts.Settings.PriceReductionResponseHours);

                e.Property(q => q.AutoCloseCLHours)
                    .HasDefaultValue(Consts.Settings.AutoCloseCLHours);

                e.Property(q => q.RoundsCount)
                    .HasDefaultValue(Consts.Settings.RoundsCount);

                e.Property(q => q.SendPriceReductionTo)
                    .HasDefaultValue(Consts.Settings.SendPriceReductionTo);

                e.ToTable("Users");
            });

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

            builder.Entity<UnitsOfMeasurement>().Property(b => b.Json)
                .HasConversion(
                    q => JsonConvert.SerializeObject(q),
                    q => JsonConvert.DeserializeObject<UomJsonData>(q));

            builder.Query<UomAutocomplete>();

            builder.Entity<NomenclatureCategory>().HasOne(q => q.Parent).WithMany(q => q.Children).HasForeignKey(q => q.ParentId);
            builder.Entity<NomenclatureCategory>().HasOne(q => q.Owner).WithMany().HasForeignKey(q => q.OwnerId);

            builder.Entity<Nomenclature>().HasOne(q => q.Category).WithMany(q => q.Nomenclatures).HasForeignKey(q => q.CategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.BatchUom).WithMany(q => q.BatchNomenclatures).HasForeignKey(q => q.BatchUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.MassUom).WithMany(q => q.MassNomenclatures).HasForeignKey(q => q.MassUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.ResourceUom).WithMany(q => q.ResourceNomenclatures).HasForeignKey(q => q.ResourceUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.ResourceBatchUom).WithMany(q => q.ResourceBatchNomenclatures).HasForeignKey(q => q.ResourceBatchUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.PackUom).WithMany().HasForeignKey(q => q.PackUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().Property(q => q.Name).IsRequired();
            builder.Entity<Nomenclature>().HasIndex(q => new { q.OwnerId, q.Name }).IsUnique().HasFilter($"{nameof(Nomenclature.IsDeleted)} = 0");
            builder.Entity<Nomenclature>().HasMany(q => q.ComparisonDataItems).WithOne(q => q.Nomenclature).HasForeignKey(q => q.NomenclatureId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NomenclatureAlternative>().HasOne(q => q.Nomenclature).WithMany(q => q.Alternatives).HasForeignKey(q => q.NomenclatureId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.BatchUom).WithMany(q => q.BatchNomenclatureAlternatives).HasForeignKey(q => q.BatchUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.MassUom).WithMany(q => q.MassNomenclatureAlternatives).HasForeignKey(q => q.MassUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.ResourceUom).WithMany(q => q.ResourceNomenclatureAlternatives).HasForeignKey(q => q.ResourceUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.ResourceBatchUom).WithMany(q => q.ResourceBatchNomenclatureAlternatives).HasForeignKey(q => q.ResourceBatchUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NomenclatureAlternative>().Property(q => q.Name).IsRequired();
            builder.Entity<NomenclatureAlternative>().HasOne(q => q.ComparisonData).WithOne().HasForeignKey<NomenclatureComparisonData>(q => q.NomenclatureAlternativeId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NomenclatureComparisonData>().Property(q => q.AdjustedNomenclatureNameWithDimensions)
                .HasComputedColumnSql($"{nameof(NomenclatureComparisonData.AdjustedNomenclatureName)} + ' ' + {nameof(NomenclatureComparisonData.NomenclatureDimensions)}");

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

            builder.Entity<PurchaseRequestAttachment>().HasOne(q => q.PurchaseRequest).WithMany().HasForeignKey(q => q.PurchaseRequestId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuotationRequest>(e =>
            {
                e.HasOne(q => q.PurchaseRequest).WithOne(q => q.QuotationRequest).HasForeignKey<QuotationRequest>(q => q.PurchaseRequestId).OnDelete(DeleteBehavior.Restrict);
                e.HasMany(q => q.Emails).WithOne(q => q.Request).HasForeignKey(q => q.RequestId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CompetitionList>().HasOne(q => q.QuotationRequest).WithOne(q => q.CompetitionList).HasForeignKey<CompetitionList>(q => q.QuotationRequestId).OnDelete(DeleteBehavior.Restrict);

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

            builder.Entity<UomConversionRate>().HasOne(q => q.FromUom).WithMany(q => q.FromConversionRates).HasForeignKey(q => q.FromUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UomConversionRate>().HasOne(q => q.ToUom).WithMany(q => q.ToConversionRates).HasForeignKey(q => q.ToUomId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Delivery>().HasMany(q => q.PurchaseRequests).WithOne(q => q.Delivery).HasForeignKey(q => q.DeliveryId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Delivery>().HasMany(q => q.QuotationRequests).WithOne(q => q.Delivery).HasForeignKey(q => q.DeliveryId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AnalysisVariant>(e =>
            {
                e.HasOne(q => q.CompetitionList).WithMany().HasForeignKey(q => q.CompetitionListId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(q => q.Owner).WithMany().HasForeignKey(q => q.OwnerId).OnDelete(DeleteBehavior.Restrict);
            });

            #region Selected supplier report

            builder.Entity<SSReport>(e =>
            {
                e.HasOne(q => q.Root).WithMany().HasForeignKey(q => q.RootId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.User).WithMany().HasForeignKey(q => q.UserId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<SSVariant>().HasOne(q => q.Report).WithMany().HasForeignKey(q => q.ReportId);
            builder.Entity<SSData>(e =>
            {
                e.HasOne(q => q.Variant).WithMany().HasForeignKey(q => q.VariantId);
                e.HasOne(q => q.Supplier).WithMany().HasForeignKey(q => q.SupplierId);
            });
            builder.Entity<SSCustomer>().HasOne(q => q.Report).WithMany().HasForeignKey(q => q.ReportId);
            builder.Entity<SSCustomerItem>().HasOne(q => q.Customer).WithMany().HasForeignKey(q => q.CustomerId);
           
            builder.Entity<SSSupplier>().HasOne(q => q.Report).WithMany().HasForeignKey(q => q.ReportId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<SSSupplierItem>().HasOne(q => q.Supplier).WithMany().HasForeignKey(q => q.SupplierId);
            
            #endregion

            builder.Entity<EmailAttachment>(e =>
            {
                e.Property(q => q.FileName).IsRequired();
                e.Property(q => q.Bytes).IsRequired();
                e.Property(q => q.ContentType).IsRequired();
                e.HasOne(q => q.ReceivedEmail).WithMany(q => q.Attachments).HasForeignKey(q => q.ReceivedEmailId).OnDelete(DeleteBehavior.Cascade);
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

            builder.Entity<AppNGram>(e =>
            {
                e.Property(q => q.Gram).IsRequired().HasMaxLength(10);
            });
            builder.Entity<NomenclatureComparisonDataNGram>(e =>
            {
                e.HasIndex(q => new { q.Gram, q.OwnerId })
                    .ForSqlServerInclude("Discriminator", nameof(NomenclatureComparisonDataNGram.NomenclatureComparisonDataId))
                    .HasName("IX_AppNGrams_Gram_OwnerId_INCL_Discriminator_NomenclatureComparisonDataId");
            });

            builder.Entity<PriceReductionEmail>(e =>
            {
                e.HasOne(q => q.User).WithMany().HasForeignKey(q => q.UserId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.SupplierOffer).WithMany().HasForeignKey(q => q.SupplierOfferId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(q => q.ContactPerson).WithMany().HasForeignKey(q => q.ContactPersonId).OnDelete(DeleteBehavior.Restrict);
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

