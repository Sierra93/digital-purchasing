using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Supplier_Inn_ChangeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_OwnerId_Inn",
                table: "Suppliers");

            migrationBuilder.AlterColumn<long>(
                name: "Inn",
                table: "Suppliers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_OwnerId_Inn",
                table: "Suppliers",
                columns: new[] { "OwnerId", "Inn" },
                unique: true,
                filter: "Name IS NOT NULL AND Inn IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Inn",
                table: "Suppliers",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
