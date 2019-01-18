using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AnalysisVariant_CompetitionListId_Nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CompetitionListId",
                table: "AnalysisVariants",
                nullable: true,
                oldClrType: typeof(Guid));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CompetitionListId",
                table: "AnalysisVariants",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
