using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AnalysisVariant_FixCompetitionListId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnalysisVariants_CompetitionListId",
                table: "AnalysisVariants");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisVariants_CompetitionListId",
                table: "AnalysisVariants",
                column: "CompetitionListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnalysisVariants_CompetitionListId",
                table: "AnalysisVariants");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisVariants_CompetitionListId",
                table: "AnalysisVariants",
                column: "CompetitionListId",
                unique: true);
        }
    }
}
