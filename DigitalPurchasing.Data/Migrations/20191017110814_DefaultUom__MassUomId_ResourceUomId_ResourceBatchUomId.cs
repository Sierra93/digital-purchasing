using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class DefaultUom__MassUomId_ResourceUomId_ResourceBatchUomId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MassUomId",
                table: "DefaultUoms",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceBatchUomId",
                table: "DefaultUoms",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceUomId",
                table: "DefaultUoms",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MassUomId",
                table: "DefaultUoms");

            migrationBuilder.DropColumn(
                name: "ResourceBatchUomId",
                table: "DefaultUoms");

            migrationBuilder.DropColumn(
                name: "ResourceUomId",
                table: "DefaultUoms");
        }
    }
}
