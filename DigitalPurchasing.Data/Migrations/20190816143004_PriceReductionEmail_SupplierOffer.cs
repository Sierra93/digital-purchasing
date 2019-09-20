using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class PriceReductionEmail_SupplierOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM PriceReductionEmails");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceReductionEmails_CompetitionLists_CompetitionListId",
                table: "PriceReductionEmails");

            migrationBuilder.RenameColumn(
                name: "CompetitionListId",
                table: "PriceReductionEmails",
                newName: "SupplierOfferId");

            migrationBuilder.RenameIndex(
                name: "IX_PriceReductionEmails_CompetitionListId",
                table: "PriceReductionEmails",
                newName: "IX_PriceReductionEmails_SupplierOfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceReductionEmails_SupplierOffers_SupplierOfferId",
                table: "PriceReductionEmails",
                column: "SupplierOfferId",
                principalTable: "SupplierOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceReductionEmails_SupplierOffers_SupplierOfferId",
                table: "PriceReductionEmails");

            migrationBuilder.RenameColumn(
                name: "SupplierOfferId",
                table: "PriceReductionEmails",
                newName: "CompetitionListId");

            migrationBuilder.RenameIndex(
                name: "IX_PriceReductionEmails_SupplierOfferId",
                table: "PriceReductionEmails",
                newName: "IX_PriceReductionEmails_CompetitionListId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceReductionEmails_CompetitionLists_CompetitionListId",
                table: "PriceReductionEmails",
                column: "CompetitionListId",
                principalTable: "CompetitionLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
