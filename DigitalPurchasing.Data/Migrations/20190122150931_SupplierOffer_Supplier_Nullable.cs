using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierOffer_Supplier_Nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SupplierId",
                table: "SupplierOffers",
                nullable: true,
                oldClrType: typeof(Guid));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SupplierId",
                table: "SupplierOffers",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
