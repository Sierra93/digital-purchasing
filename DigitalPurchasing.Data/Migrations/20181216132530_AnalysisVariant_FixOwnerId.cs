using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AnalysisVariant_FixOwnerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnalysisVariants_OwnerId",
                table: "AnalysisVariants");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisVariants_OwnerId",
                table: "AnalysisVariants",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnalysisVariants_OwnerId",
                table: "AnalysisVariants");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisVariants_OwnerId",
                table: "AnalysisVariants",
                column: "OwnerId",
                unique: true);
        }
    }
}
