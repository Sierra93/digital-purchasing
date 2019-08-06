using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierOffer_ContactPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContactPersonId",
                table: "SupplierOffers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOffers_ContactPersonId",
                table: "SupplierOffers",
                column: "ContactPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOffers_SupplierContactPersons_ContactPersonId",
                table: "SupplierOffers",
                column: "ContactPersonId",
                principalTable: "SupplierContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOffers_SupplierContactPersons_ContactPersonId",
                table: "SupplierOffers");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOffers_ContactPersonId",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "ContactPersonId",
                table: "SupplierOffers");
        }
    }
}
