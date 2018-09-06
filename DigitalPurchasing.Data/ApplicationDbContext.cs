using System;
using System.Linq;
using System.Linq.Expressions;
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
        public DbSet<NomenclatureCategory> NomenclatureCategories { get; set; }
        public DbSet<UnitsOfMeasurement> UnitsOfMeasurements { get; set; }
        public DbSet<UomConversionRate> UomConversionRates { get; set; }
        public DbSet<PurchasingRequest> PurchasingRequests { get; set; }
        public DbSet<PurchasingRequestItem> PurchasingRequestItems { get; set; }
        public DbSet<RawPurchasingRequestItem> RawPurchasingRequestItems { get; set; }
        public DbSet<PRCounter> PRCounters { get; set; }
        public DbSet<ColumnName> ColumnNames { get; set; }

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

            builder.Entity<PurchasingRequest>().HasMany(q => q.RawItems).WithOne(q => q.PurchasingRequest).HasForeignKey(q => q.PurchasingRequestId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PurchasingRequest>().HasMany(q => q.Items).WithOne(q => q.PurchasingRequest).HasForeignKey(q => q.PurchasingRequestId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UomConversionRate>().HasOne(q => q.FromUom).WithMany(q => q.FromConversionRates).HasForeignKey(q => q.FromUomId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UomConversionRate>().HasOne(q => q.ToUom).WithMany(q => q.ToConversionRates).HasForeignKey(q => q.ToUomId).OnDelete(DeleteBehavior.Restrict);

            // default filters to show company or common data
            builder.Entity<NomenclatureCategory>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<PRCounter>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<PurchasingRequest>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<Nomenclature>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<ColumnName>().HasQueryFilter(o => o.OwnerId == CompanyId);
            builder.Entity<UnitsOfMeasurement>().HasQueryFilter(o => o.OwnerId == CompanyId || o.OwnerId == null);
            builder.Entity<UomConversionRate>().HasQueryFilter(o => o.OwnerId == CompanyId || o.OwnerId == null);
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

