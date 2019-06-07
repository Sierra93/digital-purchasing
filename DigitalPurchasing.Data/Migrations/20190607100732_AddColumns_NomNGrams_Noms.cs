using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddColumns_NomNGrams_Noms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NomenclatureAlternativeId",
                table: "AppNGrams",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NomenclatureId",
                table: "AppNGrams",
                nullable: true);

            migrationBuilder.Sql(@"
update AppNGrams
set
    AppNGrams.NomenclatureId = NomenclatureComparisonDatas.NomenclatureId,
    AppNGrams.NomenclatureAlternativeId = NomenclatureComparisonDatas.NomenclatureAlternativeId
from NomenclatureComparisonDatas
where AppNGrams.NomenclatureComparisonDataId = NomenclatureComparisonDatas.Id
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomenclatureAlternativeId",
                table: "AppNGrams");

            migrationBuilder.DropColumn(
                name: "NomenclatureId",
                table: "AppNGrams");
        }
    }
}
