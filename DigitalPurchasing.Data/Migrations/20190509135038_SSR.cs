using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SSCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    InternalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SSReports_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SSSuppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    InternalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSSuppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSCustomerItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    InternalId = table.Column<Guid>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    NomenclatureId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSCustomerItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SSCustomerItems_SSCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "SSCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SSVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReportId = table.Column<Guid>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    IsSelected = table.Column<bool>(nullable: false),
                    InternalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SSVariants_SSReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "SSReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SSSupplierItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(38, 17)", nullable: false),
                    SupplierId = table.Column<Guid>(nullable: false),
                    InternalId = table.Column<Guid>(nullable: false),
                    NomenclatureId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSSupplierItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SSSupplierItems_SSSuppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SSSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SSDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VariantId = table.Column<Guid>(nullable: false),
                    SupplierId = table.Column<Guid>(nullable: false),
                    NomenclatureId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SSDatas_SSSuppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SSSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SSDatas_SSVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "SSVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SSCustomerItems_CustomerId",
                table: "SSCustomerItems",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SSDatas_SupplierId",
                table: "SSDatas",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SSDatas_VariantId",
                table: "SSDatas",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_SSReports_OwnerId",
                table: "SSReports",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SSSupplierItems_SupplierId",
                table: "SSSupplierItems",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SSVariants_ReportId",
                table: "SSVariants",
                column: "ReportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SSCustomerItems");

            migrationBuilder.DropTable(
                name: "SSDatas");

            migrationBuilder.DropTable(
                name: "SSSupplierItems");

            migrationBuilder.DropTable(
                name: "SSCustomers");

            migrationBuilder.DropTable(
                name: "SSVariants");

            migrationBuilder.DropTable(
                name: "SSSuppliers");

            migrationBuilder.DropTable(
                name: "SSReports");
        }
    }
}
