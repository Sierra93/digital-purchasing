using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class CompetitionList_QuotationRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuotationRequestId",
                table: "CompetitionLists",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionLists_QuotationRequestId",
                table: "CompetitionLists",
                column: "QuotationRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompetitionLists_QuotationRequests_QuotationRequestId",
                table: "CompetitionLists",
                column: "QuotationRequestId",
                principalTable: "QuotationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompetitionLists_QuotationRequests_QuotationRequestId",
                table: "CompetitionLists");

            migrationBuilder.DropIndex(
                name: "IX_CompetitionLists_QuotationRequestId",
                table: "CompetitionLists");

            migrationBuilder.DropColumn(
                name: "QuotationRequestId",
                table: "CompetitionLists");
        }
    }
}
