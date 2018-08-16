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

        public DbSet<Company> Companies { get; set; }
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<NomenclatureCategory> NomenclatureCategories { get; set; }
        public DbSet<UnitsOfMeasurement> UnitsOfMeasurements { get; set; }

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
            builder.Entity<Nomenclature>().HasOne(q => q.BasicUoM).WithMany(q => q.BasicNomenclatures).HasForeignKey(q => q.BasicUoMId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.MassUoM).WithMany(q => q.MassNomenclatures).HasForeignKey(q => q.MassUoMId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Nomenclature>().HasOne(q => q.CycleUoM).WithMany(q => q.CycleNomenclatures).HasForeignKey(q => q.CycleUoMId).OnDelete(DeleteBehavior.Restrict);

            // default filters to show company or common data
            builder.Entity<NomenclatureCategory>().HasQueryFilter(q => q.OwnerId == _tenantService.Get().CompanyId);
            builder.Entity<Nomenclature>().HasQueryFilter(q => q.OwnerId == _tenantService.Get().CompanyId);
            builder.Entity<UnitsOfMeasurement>().HasQueryFilter(q => q.OwnerId == _tenantService.Get().CompanyId || !q.OwnerId.HasValue);
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

