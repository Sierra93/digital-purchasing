using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddColumns_NomNGrams_Noms3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppNGrams_Gram_OwnerId_INCL_Discriminator_NomenclatureComparisonDataId",
                table: "AppNGrams");

            migrationBuilder.CreateIndex(
                name: "IX_AppNGrams_Gram_OwnerId_INCL_Discriminator_NomenclatureId",
                table: "AppNGrams",
                columns: new[] { "Gram", "OwnerId" })
                .Annotation("SqlServer:Include", new[] { "Discriminator", "NomenclatureId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppNGrams_Gram_OwnerId_INCL_Discriminator_NomenclatureId",
                table: "AppNGrams");

            migrationBuilder.CreateIndex(
                name: "IX_AppNGrams_Gram_OwnerId_INCL_Discriminator_NomenclatureComparisonDataId",
                table: "AppNGrams",
                columns: new[] { "Gram", "OwnerId" })
                .Annotation("SqlServer:Include", new[] { "Discriminator", "NomenclatureComparisonDataId" });
        }
    }
}
