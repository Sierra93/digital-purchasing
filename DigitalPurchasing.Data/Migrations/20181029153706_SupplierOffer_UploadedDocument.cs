using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierOffer_UploadedDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UploadedDocumentId",
                table: "SupplierOffers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOffers_UploadedDocumentId",
                table: "SupplierOffers",
                column: "UploadedDocumentId",
                unique: true,
                filter: "[UploadedDocumentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOffers_UploadedDocuments_UploadedDocumentId",
                table: "SupplierOffers",
                column: "UploadedDocumentId",
                principalTable: "UploadedDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOffers_UploadedDocuments_UploadedDocumentId",
                table: "SupplierOffers");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOffers_UploadedDocumentId",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "UploadedDocumentId",
                table: "SupplierOffers");
        }
    }
}
