using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Nomenclature_Name_Constraints_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_OwnerId_Name",
                table: "Nomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_OwnerId_Name",
                table: "NomenclatureAlternatives");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_OwnerId_Name",
                table: "Nomenclatures",
                columns: new[] { "OwnerId", "Name" },
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_OwnerId_NomenclatureId_Name",
                table: "NomenclatureAlternatives",
                columns: new[] { "OwnerId", "NomenclatureId", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_OwnerId_Name",
                table: "Nomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_OwnerId_NomenclatureId_Name",
                table: "NomenclatureAlternatives");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_OwnerId_Name",
                table: "Nomenclatures",
                columns: new[] { "OwnerId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_OwnerId_Name",
                table: "NomenclatureAlternatives",
                columns: new[] { "OwnerId", "Name" },
                unique: true);
        }
    }
}
