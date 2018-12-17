using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AnalysisVariant_Rework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisVariants_Analyses_AnalysisId",
                table: "AnalysisVariants");

            migrationBuilder.DropTable(
                name: "Analyses");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisVariants_AnalysisId",
                table: "AnalysisVariants");

            migrationBuilder.RenameColumn(
                name: "AnalysisId",
                table: "AnalysisVariants",
                newName: "OwnerId");

            migrationBuilder.AddColumn<Guid>(
                name: "CompetitionListId",
                table: "AnalysisVariants",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "DeliveryDateTerms",
                table: "AnalysisVariants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTerms",
                table: "AnalysisVariants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTerms",
                table: "AnalysisVariants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierCount",
                table: "AnalysisVariants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierCountType",
                table: "AnalysisVariants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "AnalysisVariants",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisVariants_CompetitionListId",
                table: "AnalysisVariants",
                column: "CompetitionListId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisVariants_OwnerId",
                table: "AnalysisVariants",
                column: "OwnerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisVariants_CompetitionLists_CompetitionListId",
                table: "AnalysisVariants",
                column: "CompetitionListId",
                principalTable: "CompetitionLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisVariants_Companies_OwnerId",
                table: "AnalysisVariants",
                column: "OwnerId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisVariants_CompetitionLists_CompetitionListId",
                table: "AnalysisVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisVariants_Companies_OwnerId",
                table: "AnalysisVariants");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisVariants_CompetitionListId",
                table: "AnalysisVariants");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisVariants_OwnerId",
                table: "AnalysisVariants");

            migrationBuilder.DropColumn(
                name: "CompetitionListId",
                table: "AnalysisVariants");

            migrationBuilder.DropColumn(
                name: "DeliveryDateTerms",
                table: "AnalysisVariants");

            migrationBuilder.DropColumn(
                name: "DeliveryTerms",
                table: "AnalysisVariants");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "AnalysisVariants");

            migrationBuilder.DropColumn(
                name: "SupplierCount",
                table: "AnalysisVariants");

            migrationBuilder.DropColumn(
                name: "SupplierCountType",
                table: "AnalysisVariants");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "AnalysisVariants");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "AnalysisVariants",
                newName: "AnalysisId");

            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompetitionListId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analyses_CompetitionLists_CompetitionListId",
                        column: x => x.CompetitionListId,
                        principalTable: "CompetitionLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Analyses_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisVariants_AnalysisId",
                table: "AnalysisVariants",
                column: "AnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_CompetitionListId",
                table: "Analyses",
                column: "CompetitionListId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_OwnerId",
                table: "Analyses",
                column: "OwnerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisVariants_Analyses_AnalysisId",
                table: "AnalysisVariants",
                column: "AnalysisId",
                principalTable: "Analyses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
