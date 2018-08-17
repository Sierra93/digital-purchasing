using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class NomenclatureRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_BasicUoMId",
                table: "Nomenclatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_CycleUoMId",
                table: "Nomenclatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_MassUoMId",
                table: "Nomenclatures");

            migrationBuilder.RenameColumn(
                name: "MassUoMId",
                table: "Nomenclatures",
                newName: "MassUomId");

            migrationBuilder.RenameColumn(
                name: "CycleUoMId",
                table: "Nomenclatures",
                newName: "ResourceUomId");

            migrationBuilder.RenameColumn(
                name: "BasicUoMId",
                table: "Nomenclatures",
                newName: "ResourceBatchUomId");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_MassUoMId",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_MassUomId");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_CycleUoMId",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_ResourceUomId");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_BasicUoMId",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_ResourceBatchUomId");

            migrationBuilder.AddColumn<Guid>(
                name: "BatchUomId",
                table: "Nomenclatures",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "MassUomValue",
                table: "Nomenclatures",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NameEng",
                table: "Nomenclatures",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ResourceUomValue",
                table: "Nomenclatures",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_BatchUomId",
                table: "Nomenclatures",
                column: "BatchUomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_BatchUomId",
                table: "Nomenclatures",
                column: "BatchUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_MassUomId",
                table: "Nomenclatures",
                column: "MassUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_ResourceBatchUomId",
                table: "Nomenclatures",
                column: "ResourceBatchUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_ResourceUomId",
                table: "Nomenclatures",
                column: "ResourceUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_BatchUomId",
                table: "Nomenclatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_MassUomId",
                table: "Nomenclatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_ResourceBatchUomId",
                table: "Nomenclatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_ResourceUomId",
                table: "Nomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_BatchUomId",
                table: "Nomenclatures");

            migrationBuilder.DropColumn(
                name: "BatchUomId",
                table: "Nomenclatures");

            migrationBuilder.DropColumn(
                name: "MassUomValue",
                table: "Nomenclatures");

            migrationBuilder.DropColumn(
                name: "NameEng",
                table: "Nomenclatures");

            migrationBuilder.DropColumn(
                name: "ResourceUomValue",
                table: "Nomenclatures");

            migrationBuilder.RenameColumn(
                name: "MassUomId",
                table: "Nomenclatures",
                newName: "MassUoMId");

            migrationBuilder.RenameColumn(
                name: "ResourceUomId",
                table: "Nomenclatures",
                newName: "CycleUoMId");

            migrationBuilder.RenameColumn(
                name: "ResourceBatchUomId",
                table: "Nomenclatures",
                newName: "BasicUoMId");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_MassUomId",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_MassUoMId");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_ResourceUomId",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_CycleUoMId");

            migrationBuilder.RenameIndex(
                name: "IX_Nomenclatures_ResourceBatchUomId",
                table: "Nomenclatures",
                newName: "IX_Nomenclatures_BasicUoMId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_BasicUoMId",
                table: "Nomenclatures",
                column: "BasicUoMId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_CycleUoMId",
                table: "Nomenclatures",
                column: "CycleUoMId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_MassUoMId",
                table: "Nomenclatures",
                column: "MassUoMId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
