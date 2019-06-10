using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddIndex_For_AppNGram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppNGrams_Gram_INCL_Discriminator_NomenclatureComparisonDataId",
                table: "AppNGrams");

            migrationBuilder.CreateIndex(
                name: "IX_AppNGrams_Gram_OwnerId_INCL_Discriminator_NomenclatureComparisonDataId",
                table: "AppNGrams",
                columns: new[] { "Gram", "OwnerId" })
                .Annotation("SqlServer:Include", new[] { "Discriminator", "NomenclatureComparisonDataId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppNGrams_Gram_OwnerId_INCL_Discriminator_NomenclatureComparisonDataId",
                table: "AppNGrams");

            migrationBuilder.CreateIndex(
                name: "IX_AppNGrams_Gram_INCL_Discriminator_NomenclatureComparisonDataId",
                table: "AppNGrams",
                column: "Gram")
                .Annotation("SqlServer:Include", new[] { "Discriminator", "NomenclatureComparisonDataId" });
        }
    }
}
