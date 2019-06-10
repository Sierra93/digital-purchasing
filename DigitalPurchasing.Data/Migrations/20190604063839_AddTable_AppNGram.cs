using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddTable_AppNGram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppNGrams",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    N = table.Column<byte>(nullable: false),
                    Gram = table.Column<string>(maxLength: 10, nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    NomenclatureComparisonDataId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNGrams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppNGrams_NomenclatureComparisonDatas_NomenclatureComparisonDataId",
                        column: x => x.NomenclatureComparisonDataId,
                        principalTable: "NomenclatureComparisonDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppNGrams_Gram_INCL_Discriminator_NomenclatureComparisonDataId",
                table: "AppNGrams",
                column: "Gram")
                .Annotation("SqlServer:Include", new[] { "Discriminator", "NomenclatureComparisonDataId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppNGrams_NomenclatureComparisonDataId",
                table: "AppNGrams",
                column: "NomenclatureComparisonDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppNGrams");
        }
    }
}
