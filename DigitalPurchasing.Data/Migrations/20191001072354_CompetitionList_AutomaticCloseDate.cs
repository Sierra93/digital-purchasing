using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class CompetitionList_AutomaticCloseDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "QuotationRequestResponseHours",
                table: "Users",
                nullable: false,
                defaultValue: 1.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldDefaultValue: 1m);

            migrationBuilder.AlterColumn<double>(
                name: "PriceReductionResponseHours",
                table: "Users",
                nullable: false,
                defaultValue: 0.5,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldDefaultValue: 0.5m);

            migrationBuilder.AlterColumn<double>(
                name: "PRDiscountPercentage",
                table: "Users",
                nullable: false,
                defaultValue: 3.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldDefaultValue: 3m);

            migrationBuilder.AlterColumn<double>(
                name: "AutoCloseCLHours",
                table: "Users",
                nullable: false,
                defaultValue: 2.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldDefaultValue: 2m);

            migrationBuilder.AddColumn<DateTime>(
                name: "AutomaticCloseDate",
                table: "CompetitionLists",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutomaticCloseDate",
                table: "CompetitionLists");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuotationRequestResponseHours",
                table: "Users",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(double),
                oldDefaultValue: 1.0);

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceReductionResponseHours",
                table: "Users",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0.5m,
                oldClrType: typeof(double),
                oldDefaultValue: 0.5);

            migrationBuilder.AlterColumn<decimal>(
                name: "PRDiscountPercentage",
                table: "Users",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 3m,
                oldClrType: typeof(double),
                oldDefaultValue: 3.0);

            migrationBuilder.AlterColumn<decimal>(
                name: "AutoCloseCLHours",
                table: "Users",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 2m,
                oldClrType: typeof(double),
                oldDefaultValue: 2.0);
        }
    }
}
