using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class RawColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RawColumnsId",
                table: "PurchasingRequests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RawColumns",
                columns: table => new
                {
                    RawColumnsId = table.Column<Guid>(nullable: false),
                    Id = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Uom = table.Column<string>(nullable: true),
                    Qty = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    Receiver = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawColumns", x => x.RawColumnsId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequests_RawColumnsId",
                table: "PurchasingRequests",
                column: "RawColumnsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingRequests_RawColumns_RawColumnsId",
                table: "PurchasingRequests",
                column: "RawColumnsId",
                principalTable: "RawColumns",
                principalColumn: "RawColumnsId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingRequests_RawColumns_RawColumnsId",
                table: "PurchasingRequests");

            migrationBuilder.DropTable(
                name: "RawColumns");

            migrationBuilder.DropIndex(
                name: "IX_PurchasingRequests_RawColumnsId",
                table: "PurchasingRequests");

            migrationBuilder.DropColumn(
                name: "RawColumnsId",
                table: "PurchasingRequests");
        }
    }
}
