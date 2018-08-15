using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class NomenclatureAndRelatedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NomenclatureCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomenclatureCategories_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NomenclatureCategories_NomenclatureCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "NomenclatureCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitsOfMeasurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsOfMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitsOfMeasurements_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Nomenclatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    BasicUoMId = table.Column<Guid>(nullable: false),
                    MassUoMId = table.Column<Guid>(nullable: false),
                    CycleUoMId = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nomenclatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nomenclatures_UnitsOfMeasurements_BasicUoMId",
                        column: x => x.BasicUoMId,
                        principalTable: "UnitsOfMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nomenclatures_NomenclatureCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "NomenclatureCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nomenclatures_UnitsOfMeasurements_CycleUoMId",
                        column: x => x.CycleUoMId,
                        principalTable: "UnitsOfMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nomenclatures_UnitsOfMeasurements_MassUoMId",
                        column: x => x.MassUoMId,
                        principalTable: "UnitsOfMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nomenclatures_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureCategories_OwnerId",
                table: "NomenclatureCategories",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureCategories_ParentId",
                table: "NomenclatureCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_BasicUoMId",
                table: "Nomenclatures",
                column: "BasicUoMId");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_CategoryId",
                table: "Nomenclatures",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_CycleUoMId",
                table: "Nomenclatures",
                column: "CycleUoMId");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_MassUoMId",
                table: "Nomenclatures",
                column: "MassUoMId");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_OwnerId",
                table: "Nomenclatures",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasurements_OwnerId",
                table: "UnitsOfMeasurements",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nomenclatures");

            migrationBuilder.DropTable(
                name: "UnitsOfMeasurements");

            migrationBuilder.DropTable(
                name: "NomenclatureCategories");
        }
    }
}
