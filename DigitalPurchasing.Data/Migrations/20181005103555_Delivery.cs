using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Delivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryId",
                table: "QuotationRequests",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryId",
                table: "PurchaseRequests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    DeliverAt = table.Column<DateTime>(nullable: false),
                    DeliveryTerms = table.Column<int>(nullable: false),
                    PaymentTerms = table.Column<int>(nullable: false),
                    PayWithinDays = table.Column<int>(nullable: false),
                    Country = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    House = table.Column<string>(nullable: true),
                    Building = table.Column<string>(nullable: true),
                    Structure = table.Column<string>(nullable: true),
                    Office = table.Column<string>(nullable: true),
                    Apartment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deliveries_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequests_DeliveryId",
                table: "QuotationRequests",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_DeliveryId",
                table: "PurchaseRequests",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OwnerId",
                table: "Deliveries",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Deliveries_DeliveryId",
                table: "PurchaseRequests",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationRequests_Deliveries_DeliveryId",
                table: "QuotationRequests",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Deliveries_DeliveryId",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationRequests_Deliveries_DeliveryId",
                table: "QuotationRequests");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_QuotationRequests_DeliveryId",
                table: "QuotationRequests");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_DeliveryId",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "QuotationRequests");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "PurchaseRequests");
        }
    }
}
