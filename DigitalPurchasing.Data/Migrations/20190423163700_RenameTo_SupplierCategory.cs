using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class RenameTo_SupplierCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierContactPersonToNomenclatureCategories");

            migrationBuilder.CreateTable(
                name: "SupplierCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NomenclatureCategoryId = table.Column<Guid>(nullable: false),
                    SupplierContactPersonId = table.Column<Guid>(nullable: false),
                    IsPrimaryContact = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierCategories_NomenclatureCategories_NomenclatureCategoryId",
                        column: x => x.NomenclatureCategoryId,
                        principalTable: "NomenclatureCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCategories_SupplierContactPersons_SupplierContactPersonId",
                        column: x => x.SupplierContactPersonId,
                        principalTable: "SupplierContactPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCategories_NomenclatureCategoryId",
                table: "SupplierCategories",
                column: "NomenclatureCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCategories_SupplierContactPersonId",
                table: "SupplierCategories",
                column: "SupplierContactPersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierCategories");

            migrationBuilder.CreateTable(
                name: "SupplierContactPersonToNomenclatureCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsPrimaryContact = table.Column<bool>(nullable: false),
                    NomenclatureCategoryId = table.Column<Guid>(nullable: false),
                    SupplierContactPersonId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierContactPersonToNomenclatureCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierContactPersonToNomenclatureCategories_NomenclatureCategories_NomenclatureCategoryId",
                        column: x => x.NomenclatureCategoryId,
                        principalTable: "NomenclatureCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierContactPersonToNomenclatureCategories_SupplierContactPersons_SupplierContactPersonId",
                        column: x => x.SupplierContactPersonId,
                        principalTable: "SupplierContactPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierContactPersonToNomenclatureCategories_NomenclatureCategoryId",
                table: "SupplierContactPersonToNomenclatureCategories",
                column: "NomenclatureCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierContactPersonToNomenclatureCategories_SupplierContactPersonId",
                table: "SupplierContactPersonToNomenclatureCategories",
                column: "SupplierContactPersonId");
        }
    }
}
