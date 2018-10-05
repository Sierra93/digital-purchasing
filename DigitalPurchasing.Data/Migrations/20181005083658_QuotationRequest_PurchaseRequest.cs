using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class QuotationRequest_PurchaseRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseRequestId",
                table: "QuotationRequests",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequests_PurchaseRequestId",
                table: "QuotationRequests",
                column: "PurchaseRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationRequests_PurchaseRequests_PurchaseRequestId",
                table: "QuotationRequests",
                column: "PurchaseRequestId",
                principalTable: "PurchaseRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationRequests_PurchaseRequests_PurchaseRequestId",
                table: "QuotationRequests");

            migrationBuilder.DropIndex(
                name: "IX_QuotationRequests_PurchaseRequestId",
                table: "QuotationRequests");

            migrationBuilder.DropColumn(
                name: "PurchaseRequestId",
                table: "QuotationRequests");
        }
    }
}
