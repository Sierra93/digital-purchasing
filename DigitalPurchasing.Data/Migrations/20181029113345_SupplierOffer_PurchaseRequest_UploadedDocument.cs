using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierOffer_PurchaseRequest_UploadedDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [PurchaseRequestItems]", true);
            migrationBuilder.Sql("DELETE FROM [CompetitionLists];DELETE FROM [QuotationRequests];DELETE FROM [PurchaseRequests];DELETE FROM [Deliveries]", true);

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_RawColumns_RawColumnsId",
                table: "PurchaseRequests");

            migrationBuilder.DropTable(
                name: "RawColumns");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_RawColumnsId",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "RawData",
                table: "PurchaseRequests");

            migrationBuilder.RenameColumn(
                name: "RawColumnsId",
                table: "PurchaseRequests",
                newName: "UploadedDocumentId");

            migrationBuilder.CreateTable(
                name: "SupplierOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    CompetitionListId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierOffers_CompetitionLists_CompetitionListId",
                        column: x => x.CompetitionListId,
                        principalTable: "CompetitionLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierOffers_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UploadedDocumentHeaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Uom = table.Column<string>(nullable: true),
                    Qty = table.Column<string>(nullable: true),
                    Price = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedDocumentHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadedDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    UploadedDocumentHeadersId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedDocuments_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UploadedDocuments_UploadedDocumentHeaders_UploadedDocumentHeadersId",
                        column: x => x.UploadedDocumentHeadersId,
                        principalTable: "UploadedDocumentHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_UploadedDocumentId",
                table: "PurchaseRequests",
                column: "UploadedDocumentId",
                unique: true,
                filter: "[UploadedDocumentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOffers_CompetitionListId",
                table: "SupplierOffers",
                column: "CompetitionListId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOffers_OwnerId",
                table: "SupplierOffers",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedDocuments_OwnerId",
                table: "UploadedDocuments",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedDocuments_UploadedDocumentHeadersId",
                table: "UploadedDocuments",
                column: "UploadedDocumentHeadersId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_UploadedDocuments_UploadedDocumentId",
                table: "PurchaseRequests",
                column: "UploadedDocumentId",
                principalTable: "UploadedDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_UploadedDocuments_UploadedDocumentId",
                table: "PurchaseRequests");

            migrationBuilder.DropTable(
                name: "SupplierOffers");

            migrationBuilder.DropTable(
                name: "UploadedDocuments");

            migrationBuilder.DropTable(
                name: "UploadedDocumentHeaders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_UploadedDocumentId",
                table: "PurchaseRequests");

            migrationBuilder.RenameColumn(
                name: "UploadedDocumentId",
                table: "PurchaseRequests",
                newName: "RawColumnsId");

            migrationBuilder.AddColumn<string>(
                name: "RawData",
                table: "PurchaseRequests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RawColumns",
                columns: table => new
                {
                    RawColumnsId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Qty = table.Column<string>(nullable: true),
                    Uom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawColumns", x => x.RawColumnsId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_RawColumnsId",
                table: "PurchaseRequests",
                column: "RawColumnsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_RawColumns_RawColumnsId",
                table: "PurchaseRequests",
                column: "RawColumnsId",
                principalTable: "RawColumns",
                principalColumn: "RawColumnsId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
