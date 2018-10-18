using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Rework_NomenclatureAlternative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "NomenclatureAlternatives");

            migrationBuilder.RenameColumn(
                name: "Uom",
                table: "NomenclatureAlternatives",
                newName: "ClientName");

            migrationBuilder.AddColumn<Guid>(
                name: "BatchUomId",
                table: "NomenclatureAlternatives",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientType",
                table: "NomenclatureAlternatives",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "MassUomId",
                table: "NomenclatureAlternatives",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MassUomValue",
                table: "NomenclatureAlternatives",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceBatchUomId",
                table: "NomenclatureAlternatives",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceUomId",
                table: "NomenclatureAlternatives",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ResourceUomValue",
                table: "NomenclatureAlternatives",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_BatchUomId",
                table: "NomenclatureAlternatives",
                column: "BatchUomId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_MassUomId",
                table: "NomenclatureAlternatives",
                column: "MassUomId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_ResourceBatchUomId",
                table: "NomenclatureAlternatives",
                column: "ResourceBatchUomId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_ResourceUomId",
                table: "NomenclatureAlternatives",
                column: "ResourceUomId");

            migrationBuilder.AddForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_BatchUomId",
                table: "NomenclatureAlternatives",
                column: "BatchUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_MassUomId",
                table: "NomenclatureAlternatives",
                column: "MassUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_ResourceBatchUomId",
                table: "NomenclatureAlternatives",
                column: "ResourceBatchUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_ResourceUomId",
                table: "NomenclatureAlternatives",
                column: "ResourceUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_BatchUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_MassUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_ResourceBatchUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_ResourceUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_BatchUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_MassUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_ResourceBatchUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_ResourceUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "BatchUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "ClientType",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "MassUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "MassUomValue",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "ResourceBatchUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "ResourceUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "ResourceUomValue",
                table: "NomenclatureAlternatives");

            migrationBuilder.RenameColumn(
                name: "ClientName",
                table: "NomenclatureAlternatives",
                newName: "Uom");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "NomenclatureAlternatives",
                nullable: true);
        }
    }
}
