using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class BasicSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [SupplierOfferItems]", true);
            migrationBuilder.Sql("DELETE FROM [SupplierOffers]", true);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "SupplierOffers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suppliers_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOffers_SupplierId",
                table: "SupplierOffers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_OwnerId",
                table: "Suppliers",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOffers_Suppliers_SupplierId",
                table: "SupplierOffers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOffers_Suppliers_SupplierId",
                table: "SupplierOffers");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOffers_SupplierId",
                table: "SupplierOffers");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "SupplierOffers");
        }
    }
}
