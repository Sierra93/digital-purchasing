using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class UomConversionRates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UomConversionRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: true),
                    FromUomId = table.Column<Guid>(nullable: false),
                    Factor = table.Column<decimal>(nullable: false),
                    ToUomId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UomConversionRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UomConversionRates_UnitsOfMeasurements_FromUomId",
                        column: x => x.FromUomId,
                        principalTable: "UnitsOfMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UomConversionRates_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UomConversionRates_UnitsOfMeasurements_ToUomId",
                        column: x => x.ToUomId,
                        principalTable: "UnitsOfMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UomConversionRates_FromUomId",
                table: "UomConversionRates",
                column: "FromUomId");

            migrationBuilder.CreateIndex(
                name: "IX_UomConversionRates_OwnerId",
                table: "UomConversionRates",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UomConversionRates_ToUomId",
                table: "UomConversionRates",
                column: "ToUomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UomConversionRates");
        }
    }
}
