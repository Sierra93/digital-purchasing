using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierOffer_Terms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationDate",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeliveryAfterConfirmationDays",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTerms",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PayWithinDays",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTerms",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriceFixedForDays",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReservedForDays",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationDate",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "DeliveryAfterConfirmationDays",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "DeliveryTerms",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "PayWithinDays",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "PriceFixedForDays",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "ReservedForDays",
                table: "SupplierOffers");
        }
    }
}
