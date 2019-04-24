using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierCategory_Primary_And_Secondary_Contact_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierCategories_SupplierContactPersons_SupplierContactPersonId",
                table: "SupplierCategories");

            migrationBuilder.DropIndex(
                name: "IX_SupplierCategories_SupplierContactPersonId",
                table: "SupplierCategories");

            migrationBuilder.DropColumn(
                name: "IsPrimaryContact",
                table: "SupplierCategories");

            migrationBuilder.DropColumn(
                name: "SupplierContactPersonId",
                table: "SupplierCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryContactPersonId",
                table: "SupplierCategories",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SecondaryContactPersonId",
                table: "SupplierCategories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCategories_PrimaryContactPersonId",
                table: "SupplierCategories",
                column: "PrimaryContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCategories_SecondaryContactPersonId",
                table: "SupplierCategories",
                column: "SecondaryContactPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierCategories_SupplierContactPersons_PrimaryContactPersonId",
                table: "SupplierCategories",
                column: "PrimaryContactPersonId",
                principalTable: "SupplierContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierCategories_SupplierContactPersons_SecondaryContactPersonId",
                table: "SupplierCategories",
                column: "SecondaryContactPersonId",
                principalTable: "SupplierContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("delete from dbo.SupplierCategories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierCategories_SupplierContactPersons_PrimaryContactPersonId",
                table: "SupplierCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierCategories_SupplierContactPersons_SecondaryContactPersonId",
                table: "SupplierCategories");

            migrationBuilder.DropIndex(
                name: "IX_SupplierCategories_PrimaryContactPersonId",
                table: "SupplierCategories");

            migrationBuilder.DropIndex(
                name: "IX_SupplierCategories_SecondaryContactPersonId",
                table: "SupplierCategories");

            migrationBuilder.DropColumn(
                name: "PrimaryContactPersonId",
                table: "SupplierCategories");

            migrationBuilder.DropColumn(
                name: "SecondaryContactPersonId",
                table: "SupplierCategories");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimaryContact",
                table: "SupplierCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierContactPersonId",
                table: "SupplierCategories",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCategories_SupplierContactPersonId",
                table: "SupplierCategories",
                column: "SupplierContactPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierCategories_SupplierContactPersons_SupplierContactPersonId",
                table: "SupplierCategories",
                column: "SupplierContactPersonId",
                principalTable: "SupplierContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
