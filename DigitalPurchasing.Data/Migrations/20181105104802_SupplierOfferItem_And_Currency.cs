using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierOfferItem_And_Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierOfferItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    SupplierOfferId = table.Column<Guid>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    RawCode = table.Column<string>(nullable: true),
                    RawName = table.Column<string>(nullable: true),
                    RawUom = table.Column<string>(nullable: true),
                    RawQty = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    RawPrice = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    CurrencyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierOfferItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierOfferItems_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierOfferItems_SupplierOffers_SupplierOfferId",
                        column: x => x.SupplierOfferId,
                        principalTable: "SupplierOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CreatedOn", "Name" },
                values: new object[] { new Guid("77fd8325-b90c-42ae-9e03-a2f92ea6688e"), new DateTime(2018, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "RUB" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CreatedOn", "Name" },
                values: new object[] { new Guid("bb4ebd3e-ba24-4c86-88c9-40306a171349"), new DateTime(2018, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CreatedOn", "Name" },
                values: new object[] { new Guid("00a0127e-f9dc-4b1a-b4db-13c014e827ec"), new DateTime(2018, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "EUR" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferItems_CurrencyId",
                table: "SupplierOfferItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferItems_SupplierOfferId",
                table: "SupplierOfferItems",
                column: "SupplierOfferId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierOfferItems");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
