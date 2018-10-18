using System;
using System.Linq;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
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
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<NomenclatureAlternative> NomenclatureAlternatives { get; set; }
        public DbSet<NomenclatureCategory> NomenclatureCategories { get; set; }
        public DbSet<UnitsOfMeasurement> UnitsOfMeasurements { get; set; }
        public DbSet<UomConversionRate> UomConversionRates { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public DbSet<QuotationRequest> QuotationRequests { get; set; }
        public DbSet<PRCounter> PRCounters { get; set; }
        public DbSet<QRCounter> QRCounters { get; set; }
        public DbSet<ColumnName> ColumnNames { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

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

            builder.Entity<PurchaseRequest>().HasMany(q => q.Items).WithOne(q => q.PurchaseRequest).HasForeignKey(q => q.PurchaseRequestId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PurchaseRequestItem>().HasOne(q => q.Nomenclature).WithMany(q => q.PurchasingRequestItems).HasForeignKey(q => q.NomenclatureId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PurchaseRequestItem>().HasOne(q => q.RawUomMatch).WithMany(q => q.PurchasingRequestItems).HasForeignKey(q => q.RawUomMatchId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuotationRequest>().HasOne(q => q.PurchaseRequest).WithOne().HasForeignKey<QuotationRequest>(q => q.PurchaseRequestId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UomConversionRate>().HasOne(q => q.FromUom).WithMany(q => q.FromConversionRates).HasForeignKey(q => q.FromUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UomConversionRate>().HasOne(q => q.ToUom).WithMany(q => q.ToConversionRates).HasForeignKey(q => q.ToUomId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Delivery>().HasMany(q => q.PurchaseRequests).WithOne(q => q.Delivery).HasForeignKey(q => q.DeliveryId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Delivery>().HasMany(q => q.QuotationRequests).WithOne(q => q.Delivery).HasForeignKey(q => q.DeliveryId).OnDelete(DeleteBehavior.Restrict);

            // default filters to show company or common data
            builder.Entity<PurchaseRequest>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<QuotationRequest>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<Delivery>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<PRCounter>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<QRCounter>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<Nomenclature>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<NomenclatureAlternative>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<NomenclatureCategory>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<ColumnName>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<UnitsOfMeasurement>().HasQueryFilter(o => o.OwnerId == CompanyId || o.OwnerId == null);
            builder.Entity<UomConversionRate>().HasQueryFilter(o => o.OwnerId == CompanyId || o.OwnerId == null);

            var createdOn = new DateTime(2018, 09, 27, 0, 0, 0, DateTimeKind.Utc);

            builder.Entity<UnitsOfMeasurement>().HasData(
                new UnitsOfMeasurement { Id = new Guid("0a45a476e69f4ebbbb540ae92c88e64b"), OwnerId = null, Name = "шт", CreatedOn = createdOn },
                new UnitsOfMeasurement { Id = new Guid("5d8949a33c3d44c0b22ee9ec1881faf0"), OwnerId = null, Name = "тыс шт", CreatedOn = createdOn },
                new UnitsOfMeasurement { Id = new Guid("e6fe6c76ef6841ddbe4a07f46f274334"), OwnerId = null, Name = "кг", CreatedOn = createdOn }
            );

            builder.Entity<UomConversionRate>().HasData(
                new UomConversionRate
                {
                    Id = new Guid("f57c690afbb147e29ab01472a514d88f"),
                    OwnerId = null,
                    FromUomId = new Guid("5d8949a33c3d44c0b22ee9ec1881faf0"),
                    Factor = 1000,
                    ToUomId = new Guid("0a45a476e69f4ebbbb540ae92c88e64b"),
                    CreatedOn = createdOn
                }
            );
        }

        public override int SaveChanges()
        {
            var companyId = _tenantService.Get().CompanyId;

            var addedEntities = ChangeTracker.Entries().Where(c => c.State == EntityState.Added).Select(q => q.Entity).ToList();

            foreach (var entity in addedEntities.OfType<IHaveOwner>())
            {
                entity.OwnerId = companyId;
            }

            foreach (var entity in addedEntities.OfType<IMayHaveOwner>())
            {
                entity.OwnerId = companyId;
            }

            return base.SaveChanges();
        }
    }
}

