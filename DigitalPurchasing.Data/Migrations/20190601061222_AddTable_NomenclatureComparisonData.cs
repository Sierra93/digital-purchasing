using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddTable_NomenclatureComparisonData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NomenclatureComparisonDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    AdjustedNomenclatureName = table.Column<string>(nullable: true),
                    NomenclatureDimensions = table.Column<string>(nullable: true),
                    AdjustedNomenclatureNameWithDimensions = table.Column<string>(nullable: true, computedColumnSql: "AdjustedNomenclatureName + ' ' + NomenclatureDimensions"),
                    AdjustedNomenclatureDigits = table.Column<string>(nullable: true),
                    NomenclatureId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureComparisonDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomenclatureComparisonDatas_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureComparisonDatas_NomenclatureId",
                table: "NomenclatureComparisonDatas",
                column: "NomenclatureId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NomenclatureComparisonDatas");
        }
    }
}
