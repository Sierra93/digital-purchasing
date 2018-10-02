using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class NomenclatureAlternative_Uom_Code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "NomenclatureAlternatives",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uom",
                table: "NomenclatureAlternatives",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "Uom",
                table: "NomenclatureAlternatives");
        }
    }
}
