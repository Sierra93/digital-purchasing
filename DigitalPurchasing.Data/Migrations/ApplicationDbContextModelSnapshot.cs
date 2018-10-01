﻿// <auto-generated />
using System;
using DigitalPurchasing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalPurchasing.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DigitalPurchasing.Models.ColumnName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Names");

                    b.Property<Guid>("OwnerId");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("ColumnNames");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.RoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<Guid>("CompanyId");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.UserLogin", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<Guid>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.UserRole", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.UserToken", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Nomenclature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BatchUomId");

                    b.Property<Guid>("CategoryId");

                    b.Property<string>("Code");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<bool>("IsDeleted");

                    b.Property<Guid>("MassUomId");

                    b.Property<decimal>("MassUomValue")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<string>("Name");

                    b.Property<string>("NameEng");

                    b.Property<Guid>("OwnerId");

                    b.Property<Guid>("ResourceBatchUomId");

                    b.Property<Guid>("ResourceUomId");

                    b.Property<decimal>("ResourceUomValue")
                        .HasColumnType("decimal(18, 4)");

                    b.HasKey("Id");

                    b.HasIndex("BatchUomId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("MassUomId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ResourceBatchUomId");

                    b.HasIndex("ResourceUomId");

                    b.ToTable("Nomenclatures");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.NomenclatureAlternative", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("CustomerName");

                    b.Property<string>("Name");

                    b.Property<Guid>("NomenclatureId");

                    b.Property<Guid>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("NomenclatureId");

                    b.HasIndex("OwnerId");

                    b.ToTable("NomenclatureAlternatives");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.NomenclatureCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.Property<Guid>("OwnerId");

                    b.Property<Guid?>("ParentId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentId");

                    b.ToTable("NomenclatureCategories");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.PRCounter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("CurrentId")
                        .IsConcurrencyToken();

                    b.Property<Guid>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("PRCounters");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.PurchasingRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CompanyName");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("CustomerName");

                    b.Property<Guid>("OwnerId");

                    b.Property<int>("PublicId");

                    b.Property<Guid?>("RawColumnsId");

                    b.Property<string>("RawData");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("RawColumnsId");

                    b.ToTable("PurchasingRequests");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.PurchasingRequestItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("CommonFactor")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<decimal>("NomenclatureFactor")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<Guid?>("NomenclatureId");

                    b.Property<int>("Position");

                    b.Property<Guid>("PurchasingRequestId");

                    b.Property<string>("RawCode");

                    b.Property<string>("RawName");

                    b.Property<decimal>("RawQty")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<string>("RawUom");

                    b.Property<Guid?>("RawUomMatchId");

                    b.HasKey("Id");

                    b.HasIndex("NomenclatureId");

                    b.HasIndex("PurchasingRequestId");

                    b.HasIndex("RawUomMatchId");

                    b.ToTable("PurchasingRequestItems");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.QRCounter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("CurrentId")
                        .IsConcurrencyToken();

                    b.Property<Guid>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("QRCounters");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.QuotationRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid>("OwnerId");

                    b.Property<int>("PublicId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("QuotationRequests");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.RawColumns", b =>
                {
                    b.Property<Guid>("RawColumnsId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.Property<string>("Qty");

                    b.Property<string>("Uom");

                    b.HasKey("RawColumnsId");

                    b.ToTable("RawColumns");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.UnitsOfMeasurement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.Property<Guid?>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("UnitsOfMeasurements");

                    b.HasData(
                        new { Id = new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"), CreatedOn = new DateTime(2018, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), Name = "шт" },
                        new { Id = new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"), CreatedOn = new DateTime(2018, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), Name = "тыс шт" },
                        new { Id = new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"), CreatedOn = new DateTime(2018, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), Name = "кг" }
                    );
                });

            modelBuilder.Entity("DigitalPurchasing.Models.UomConversionRate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<decimal>("Factor")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<Guid>("FromUomId");

                    b.Property<Guid?>("NomenclatureId");

                    b.Property<Guid?>("OwnerId");

                    b.Property<Guid>("ToUomId");

                    b.HasKey("Id");

                    b.HasIndex("FromUomId");

                    b.HasIndex("NomenclatureId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ToUomId");

                    b.ToTable("UomConversionRates");

                    b.HasData(
                        new { Id = new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"), CreatedOn = new DateTime(2018, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), Factor = 1000m, FromUomId = new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"), ToUomId = new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b") }
                    );
                });

            modelBuilder.Entity("DigitalPurchasing.Models.ColumnName", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.RoleClaim", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Identity.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.User", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Company")
                        .WithMany("Users")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.UserClaim", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Identity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.UserLogin", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Identity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.UserRole", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Identity.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.Identity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Identity.UserToken", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Identity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Nomenclature", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "BatchUom")
                        .WithMany("BatchNomenclatures")
                        .HasForeignKey("BatchUomId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.NomenclatureCategory", "Category")
                        .WithMany("Nomenclatures")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "MassUom")
                        .WithMany("MassNomenclatures")
                        .HasForeignKey("MassUomId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "ResourceBatchUom")
                        .WithMany("ResourceBatchNomenclatures")
                        .HasForeignKey("ResourceBatchUomId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "ResourceUom")
                        .WithMany("ResourceNomenclatures")
                        .HasForeignKey("ResourceUomId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.NomenclatureAlternative", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Nomenclature", "Nomenclature")
                        .WithMany("Alternatives")
                        .HasForeignKey("NomenclatureId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.NomenclatureCategory", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany("NomenclatureCategories")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.NomenclatureCategory", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.PRCounter", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany("PRCounters")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.PurchasingRequest", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.RawColumns", "RawColumns")
                        .WithMany()
                        .HasForeignKey("RawColumnsId");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.PurchasingRequestItem", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Nomenclature", "Nomenclature")
                        .WithMany("PurchasingRequestItems")
                        .HasForeignKey("NomenclatureId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.PurchasingRequest", "PurchasingRequest")
                        .WithMany("Items")
                        .HasForeignKey("PurchasingRequestId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "RawUomMatch")
                        .WithMany("PurchasingRequestItems")
                        .HasForeignKey("RawUomMatchId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.QRCounter", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.QuotationRequest", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.UnitsOfMeasurement", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany("UnitsOfMeasurements")
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.UomConversionRate", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "FromUom")
                        .WithMany("FromConversionRates")
                        .HasForeignKey("FromUomId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.Nomenclature", "Nomenclature")
                        .WithMany("ConversionRates")
                        .HasForeignKey("NomenclatureId");

                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "ToUom")
                        .WithMany("ToConversionRates")
                        .HasForeignKey("ToUomId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
