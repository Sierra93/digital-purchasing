using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class FixTypoInPR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchasingRequestItems");

            migrationBuilder.DropTable(
                name: "PurchasingRequests");

            migrationBuilder.CreateTable(
                name: "PurchaseRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    PublicId = table.Column<int>(nullable: false),
                    CompanyName = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    RawData = table.Column<string>(nullable: true),
                    RawColumnsId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_RawColumns_RawColumnsId",
                        column: x => x.RawColumnsId,
                        principalTable: "RawColumns",
                        principalColumn: "RawColumnsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    PurchaseRequestId = table.Column<Guid>(nullable: false),
                    NomenclatureId = table.Column<Guid>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    RawCode = table.Column<string>(nullable: true),
                    RawName = table.Column<string>(nullable: true),
                    RawUom = table.Column<string>(nullable: true),
                    RawQty = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    RawUomMatchId = table.Column<Guid>(nullable: true),
                    CommonFactor = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    NomenclatureFactor = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestItems_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestItems_PurchaseRequests_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalTable: "PurchaseRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestItems_UnitsOfMeasurements_RawUomMatchId",
                        column: x => x.RawUomMatchId,
                        principalTable: "UnitsOfMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestItems_NomenclatureId",
                table: "PurchaseRequestItems",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestItems_PurchaseRequestId",
                table: "PurchaseRequestItems",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestItems_RawUomMatchId",
                table: "PurchaseRequestItems",
                column: "RawUomMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_OwnerId",
                table: "PurchaseRequests",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_RawColumnsId",
                table: "PurchaseRequests",
                column: "RawColumnsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseRequestItems");

            migrationBuilder.DropTable(
                name: "PurchaseRequests");

            migrationBuilder.CreateTable(
                name: "PurchasingRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompanyName = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CustomerName = table.Column<string>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false),
                    PublicId = table.Column<int>(nullable: false),
                    RawColumnsId = table.Column<Guid>(nullable: true),
                    RawData = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasingRequests_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchasingRequests_RawColumns_RawColumnsId",
                        column: x => x.RawColumnsId,
                        principalTable: "RawColumns",
                        principalColumn: "RawColumnsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchasingRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CommonFactor = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    NomenclatureFactor = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    NomenclatureId = table.Column<Guid>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    PurchasingRequestId = table.Column<Guid>(nullable: false),
                    RawCode = table.Column<string>(nullable: true),
                    RawName = table.Column<string>(nullable: true),
                    RawQty = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    RawUom = table.Column<string>(nullable: true),
                    RawUomMatchId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasingRequestItems_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                        column: x => x.PurchasingRequestId,
                        principalTable: "PurchasingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchasingRequestItems_UnitsOfMeasurements_RawUomMatchId",
                        column: x => x.RawUomMatchId,
                        principalTable: "UnitsOfMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequestItems_NomenclatureId",
                table: "PurchasingRequestItems",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequestItems_PurchasingRequestId",
                table: "PurchasingRequestItems",
                column: "PurchasingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequestItems_RawUomMatchId",
                table: "PurchasingRequestItems",
                column: "RawUomMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequests_OwnerId",
                table: "PurchasingRequests",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequests_RawColumnsId",
                table: "PurchasingRequests",
                column: "RawColumnsId");
        }
    }
}
