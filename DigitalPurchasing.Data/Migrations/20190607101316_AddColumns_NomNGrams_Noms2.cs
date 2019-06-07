using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddColumns_NomNGrams_Noms2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AppNGrams_NomenclatureAlternativeId",
                table: "AppNGrams",
                column: "NomenclatureAlternativeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppNGrams_NomenclatureId",
                table: "AppNGrams",
                column: "NomenclatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppNGrams_NomenclatureAlternatives_NomenclatureAlternativeId",
                table: "AppNGrams",
                column: "NomenclatureAlternativeId",
                principalTable: "NomenclatureAlternatives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppNGrams_Nomenclatures_NomenclatureId",
                table: "AppNGrams",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppNGrams_NomenclatureAlternatives_NomenclatureAlternativeId",
                table: "AppNGrams");

            migrationBuilder.DropForeignKey(
                name: "FK_AppNGrams_Nomenclatures_NomenclatureId",
                table: "AppNGrams");

            migrationBuilder.DropIndex(
                name: "IX_AppNGrams_NomenclatureAlternativeId",
                table: "AppNGrams");

            migrationBuilder.DropIndex(
                name: "IX_AppNGrams_NomenclatureId",
                table: "AppNGrams");
        }
    }
}
