using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class User__Additional_settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PRDiscountPercentage",
                table: "Users",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 3m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 3m);

            migrationBuilder.AddColumn<decimal>(
                name: "AutoCloseCLHours",
                table: "Users",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 2m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceReductionResponseHours",
                table: "Users",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0.5m);

            migrationBuilder.AddColumn<decimal>(
                name: "QuotationRequestResponseHours",
                table: "Users",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<int>(
                name: "RoundsCount",
                table: "Users",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<byte>(
                name: "SendPriceReductionTo",
                table: "Users",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoCloseCLHours",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PriceReductionResponseHours",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QuotationRequestResponseHours",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoundsCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SendPriceReductionTo",
                table: "Users");

            migrationBuilder.AlterColumn<decimal>(
                name: "PRDiscountPercentage",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 3m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldDefaultValue: 3m);
        }
    }
}
