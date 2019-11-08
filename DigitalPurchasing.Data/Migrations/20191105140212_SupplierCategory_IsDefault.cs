using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierCategory_IsDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "SupplierCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE [SupplierCategories] SET [IsDefault] = 1 FROM [SupplierCategories] AS [SC] INNER JOIN [Suppliers] AS [S] ON [S].[CategoryId] = [SC].[NomenclatureCategoryId] WHERE [CategoryId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "SupplierCategories");
        }
    }
}
