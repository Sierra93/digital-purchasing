using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class FixPurchasingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                table: "PurchasingRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RawPurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                table: "RawPurchasingRequestItems");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "PurchasingRequests",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequests_OwnerId",
                table: "PurchasingRequests",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                table: "PurchasingRequestItems",
                column: "PurchasingRequestId",
                principalTable: "PurchasingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingRequests_Companies_OwnerId",
                table: "PurchasingRequests",
                column: "OwnerId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RawPurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                table: "RawPurchasingRequestItems",
                column: "PurchasingRequestId",
                principalTable: "PurchasingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                table: "PurchasingRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingRequests_Companies_OwnerId",
                table: "PurchasingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RawPurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                table: "RawPurchasingRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchasingRequests_OwnerId",
                table: "PurchasingRequests");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "PurchasingRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                table: "PurchasingRequestItems",
                column: "PurchasingRequestId",
                principalTable: "PurchasingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RawPurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                table: "RawPurchasingRequestItems",
                column: "PurchasingRequestId",
                principalTable: "PurchasingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
