using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddTable_SupplierContactPersonToNomenclatureCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierContactPersonToNomenclatureCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NomenclatureCategoryId = table.Column<Guid>(nullable: false),
                    SupplierContactPersonId = table.Column<Guid>(nullable: false),
                    IsPrimaryContact = table.Column<bool>(nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierContactPersonToNomenclatureCategories");
        }
    }
}
