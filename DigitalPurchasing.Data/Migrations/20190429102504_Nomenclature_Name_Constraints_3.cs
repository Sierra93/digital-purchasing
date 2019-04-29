using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Nomenclature_Name_Constraints_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_OwnerId_NomenclatureId_Name",
                table: "NomenclatureAlternatives");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NomenclatureAlternatives",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_OwnerId",
                table: "NomenclatureAlternatives",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_OwnerId",
                table: "NomenclatureAlternatives");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NomenclatureAlternatives",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_OwnerId_NomenclatureId_Name",
                table: "NomenclatureAlternatives",
                columns: new[] { "OwnerId", "NomenclatureId", "Name" },
                unique: true);
        }
    }
}
