using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class PriceReductionEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceReductionEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CompetitionListId = table.Column<Guid>(nullable: false),
                    ContactPersonId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceReductionEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceReductionEmails_CompetitionLists_CompetitionListId",
                        column: x => x.CompetitionListId,
                        principalTable: "CompetitionLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceReductionEmails_SupplierContactPersons_ContactPersonId",
                        column: x => x.ContactPersonId,
                        principalTable: "SupplierContactPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceReductionEmails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceReductionEmails_CompetitionListId",
                table: "PriceReductionEmails",
                column: "CompetitionListId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceReductionEmails_ContactPersonId",
                table: "PriceReductionEmails",
                column: "ContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceReductionEmails_UserId",
                table: "PriceReductionEmails",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceReductionEmails");
        }
    }
}
