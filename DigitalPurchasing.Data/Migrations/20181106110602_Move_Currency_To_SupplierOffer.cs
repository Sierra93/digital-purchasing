using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Move_Currency_To_SupplierOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOfferItems_Currencies_CurrencyId",
                table: "SupplierOfferItems");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOfferItems_CurrencyId",
                table: "SupplierOfferItems");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "SupplierOfferItems");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: new Guid("77fd8325b90c42ae9e03a2f92ea6688e"));

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOffers_CurrencyId",
                table: "SupplierOffers",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOffers_Currencies_CurrencyId",
                table: "SupplierOffers",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOffers_Currencies_CurrencyId",
                table: "SupplierOffers");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOffers_CurrencyId",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "SupplierOffers");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "SupplierOfferItems",
                nullable: false,
                defaultValue: new Guid("77fd8325b90c42ae9e03a2f92ea6688e"));

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferItems_CurrencyId",
                table: "SupplierOfferItems",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOfferItems_Currencies_CurrencyId",
                table: "SupplierOfferItems",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
