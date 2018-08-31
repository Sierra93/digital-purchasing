using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ColumnsFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "RawColumns");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RawColumns");

            migrationBuilder.DropColumn(
                name: "Receiver",
                table: "RawColumns");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "RawColumns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "RawColumns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Receiver",
                table: "RawColumns",
                nullable: true);
        }
    }
}
