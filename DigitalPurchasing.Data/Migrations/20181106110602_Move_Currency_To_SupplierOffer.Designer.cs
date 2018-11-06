﻿// <auto-generated />
using System;
using DigitalPurchasing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalPurchasing.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20181106110602_Move_Currency_To_SupplierOffer")]
    partial class Move_Currency_To_SupplierOffer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
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

            modelBuilder.Entity("DigitalPurchasing.Models.CompetitionList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid>("OwnerId");

                    b.Property<int>("PublicId");

                    b.Property<Guid>("QuotationRequestId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("QuotationRequestId")
                        .IsUnique();

                    b.ToTable("CompetitionLists");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Counters.CLCounter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("CurrentId")
                        .IsConcurrencyToken();

                    b.Property<Guid>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("CLCounters");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Counters.PRCounter", b =>
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

            modelBuilder.Entity("DigitalPurchasing.Models.Counters.QRCounter", b =>
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

            modelBuilder.Entity("DigitalPurchasing.Models.Counters.SOCounter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("CurrentId")
                        .IsConcurrencyToken();

                    b.Property<Guid>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("SOCounters");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Currencies");

                    b.HasData(
                        new { Id = new Guid("77fd8325-b90c-42ae-9e03-a2f92ea6688e"), CreatedOn = new DateTime(2018, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), Name = "RUB" },
                        new { Id = new Guid("bb4ebd3e-ba24-4c86-88c9-40306a171349"), CreatedOn = new DateTime(2018, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), Name = "USD" },
                        new { Id = new Guid("00a0127e-f9dc-4b1a-b4db-13c014e827ec"), CreatedOn = new DateTime(2018, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), Name = "EUR" }
                    );
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Delivery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Building");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime>("DeliverAt");

                    b.Property<int>("DeliveryTerms");

                    b.Property<string>("House");

                    b.Property<string>("OfficeOrApartment");

                    b.Property<Guid>("OwnerId");

                    b.Property<int>("PayWithinDays");

                    b.Property<int>("PaymentTerms");

                    b.Property<string>("Street");

                    b.Property<string>("Structure");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Deliveries");
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

                    b.Property<Guid?>("BatchUomId");

                    b.Property<string>("ClientName");

                    b.Property<int>("ClientType");

                    b.Property<string>("Code");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid?>("MassUomId");

                    b.Property<decimal>("MassUomValue")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<string>("Name");

                    b.Property<Guid>("NomenclatureId");

                    b.Property<Guid>("OwnerId");

                    b.Property<Guid?>("ResourceBatchUomId");

                    b.Property<Guid?>("ResourceUomId");

                    b.Property<decimal>("ResourceUomValue")
                        .HasColumnType("decimal(18, 4)");

                    b.HasKey("Id");

                    b.HasIndex("BatchUomId");

                    b.HasIndex("MassUomId");

                    b.HasIndex("NomenclatureId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ResourceBatchUomId");

                    b.HasIndex("ResourceUomId");

                    b.ToTable("NomenclatureAlternatives");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.NomenclatureCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name");

                    b.Property<Guid>("OwnerId");

                    b.Property<Guid?>("ParentId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentId");

                    b.ToTable("NomenclatureCategories");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.PurchaseRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CompanyName");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("CustomerName");

                    b.Property<Guid?>("DeliveryId");

                    b.Property<Guid>("OwnerId");

                    b.Property<int>("PublicId");

                    b.Property<int>("Status");

                    b.Property<Guid?>("UploadedDocumentId");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("UploadedDocumentId")
                        .IsUnique()
                        .HasFilter("[UploadedDocumentId] IS NOT NULL");

                    b.ToTable("PurchaseRequests");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.PurchaseRequestItem", b =>
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

                    b.Property<Guid>("PurchaseRequestId");

                    b.Property<string>("RawCode");

                    b.Property<string>("RawName");

                    b.Property<decimal>("RawQty")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<string>("RawUom");

                    b.Property<Guid?>("RawUomMatchId");

                    b.HasKey("Id");

                    b.HasIndex("NomenclatureId");

                    b.HasIndex("PurchaseRequestId");

                    b.HasIndex("RawUomMatchId");

                    b.ToTable("PurchaseRequestItems");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.QuotationRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid?>("DeliveryId");

                    b.Property<Guid>("OwnerId");

                    b.Property<int>("PublicId");

                    b.Property<Guid>("PurchaseRequestId");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("PurchaseRequestId")
                        .IsUnique();

                    b.ToTable("QuotationRequests");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.SupplierOffer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CompetitionListId");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid>("CurrencyId");

                    b.Property<Guid>("OwnerId");

                    b.Property<int>("PublicId");

                    b.Property<int>("Status");

                    b.Property<string>("SupplierName");

                    b.Property<Guid?>("UploadedDocumentId");

                    b.HasKey("Id");

                    b.HasIndex("CompetitionListId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("UploadedDocumentId")
                        .IsUnique()
                        .HasFilter("[UploadedDocumentId] IS NOT NULL");

                    b.ToTable("SupplierOffers");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.SupplierOfferItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("Position");

                    b.Property<string>("RawCode");

                    b.Property<string>("RawName");

                    b.Property<decimal>("RawPrice")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("RawQty")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<string>("RawUom");

                    b.Property<Guid>("SupplierOfferId");

                    b.HasKey("Id");

                    b.HasIndex("SupplierOfferId");

                    b.ToTable("SupplierOfferItems");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.UnitsOfMeasurement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name");

                    b.Property<Guid>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("UnitsOfMeasurements");
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

                    b.Property<Guid>("OwnerId");

                    b.Property<Guid>("ToUomId");

                    b.HasKey("Id");

                    b.HasIndex("FromUomId");

                    b.HasIndex("NomenclatureId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ToUomId");

                    b.ToTable("UomConversionRates");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.UploadedDocument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Data");

                    b.Property<Guid>("OwnerId");

                    b.Property<Guid>("UploadedDocumentHeadersId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("UploadedDocumentHeadersId");

                    b.ToTable("UploadedDocuments");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.UploadedDocumentHeaders", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.Property<string>("Price");

                    b.Property<string>("Qty");

                    b.Property<string>("Uom");

                    b.HasKey("Id");

                    b.ToTable("UploadedDocumentHeaders");
                });

            modelBuilder.Entity("DigitalPurchasing.Models.ColumnName", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.CompetitionList", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.QuotationRequest", "QuotationRequest")
                        .WithOne()
                        .HasForeignKey("DigitalPurchasing.Models.CompetitionList", "QuotationRequestId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Counters.CLCounter", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Counters.PRCounter", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany("PRCounters")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Counters.QRCounter", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Counters.SOCounter", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.Delivery", b =>
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
                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "BatchUom")
                        .WithMany("BatchNomenclatureAlternatives")
                        .HasForeignKey("BatchUomId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "MassUom")
                        .WithMany("MassNomenclatureAlternatives")
                        .HasForeignKey("MassUomId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.Nomenclature", "Nomenclature")
                        .WithMany("Alternatives")
                        .HasForeignKey("NomenclatureId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "ResourceBatchUom")
                        .WithMany("ResourceBatchNomenclatureAlternatives")
                        .HasForeignKey("ResourceBatchUomId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "ResourceUom")
                        .WithMany("ResourceNomenclatureAlternatives")
                        .HasForeignKey("ResourceUomId")
                        .OnDelete(DeleteBehavior.Restrict);
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

            modelBuilder.Entity("DigitalPurchasing.Models.PurchaseRequest", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Delivery", "Delivery")
                        .WithMany("PurchaseRequests")
                        .HasForeignKey("DeliveryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.UploadedDocument", "UploadedDocument")
                        .WithOne()
                        .HasForeignKey("DigitalPurchasing.Models.PurchaseRequest", "UploadedDocumentId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.PurchaseRequestItem", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Nomenclature", "Nomenclature")
                        .WithMany("PurchasingRequestItems")
                        .HasForeignKey("NomenclatureId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.PurchaseRequest", "PurchaseRequest")
                        .WithMany("Items")
                        .HasForeignKey("PurchaseRequestId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "RawUomMatch")
                        .WithMany("PurchasingRequestItems")
                        .HasForeignKey("RawUomMatchId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.QuotationRequest", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Delivery", "Delivery")
                        .WithMany("QuotationRequests")
                        .HasForeignKey("DeliveryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.PurchaseRequest", "PurchaseRequest")
                        .WithOne()
                        .HasForeignKey("DigitalPurchasing.Models.QuotationRequest", "PurchaseRequestId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.SupplierOffer", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.CompetitionList", "CompetitionList")
                        .WithMany("SupplierOffers")
                        .HasForeignKey("CompetitionListId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DigitalPurchasing.Models.UploadedDocument", "UploadedDocument")
                        .WithOne()
                        .HasForeignKey("DigitalPurchasing.Models.SupplierOffer", "UploadedDocumentId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.SupplierOfferItem", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.SupplierOffer", "SupplierOffer")
                        .WithMany()
                        .HasForeignKey("SupplierOfferId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.UnitsOfMeasurement", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany("UnitsOfMeasurements")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
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
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.UnitsOfMeasurement", "ToUom")
                        .WithMany("ToConversionRates")
                        .HasForeignKey("ToUomId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DigitalPurchasing.Models.UploadedDocument", b =>
                {
                    b.HasOne("DigitalPurchasing.Models.Company", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DigitalPurchasing.Models.UploadedDocumentHeaders", "Headers")
                        .WithMany()
                        .HasForeignKey("UploadedDocumentHeadersId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
