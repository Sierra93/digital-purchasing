using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class PurchasingRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchasingRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    RawData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchasingRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    PurchasingRequestId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                        column: x => x.PurchasingRequestId,
                        principalTable: "PurchasingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RawPurchasingRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    PurchasingRequestId = table.Column<Guid>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Uom = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawPurchasingRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RawPurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                        column: x => x.PurchasingRequestId,
                        principalTable: "PurchasingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequestItems_PurchasingRequestId",
                table: "PurchasingRequestItems",
                column: "PurchasingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RawPurchasingRequestItems_PurchasingRequestId",
                table: "RawPurchasingRequestItems",
                column: "PurchasingRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchasingRequestItems");

            migrationBuilder.DropTable(
                name: "RawPurchasingRequestItems");

            migrationBuilder.DropTable(
                name: "PurchasingRequests");
        }
    }
}
