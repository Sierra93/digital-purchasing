using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Delete_SelectedSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SelectedSuppliers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SelectedSuppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    NomenclatureId = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    RootId = table.Column<Guid>(nullable: false),
                    SupplierId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedSuppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectedSuppliers_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SelectedSuppliers_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SelectedSuppliers_Roots_RootId",
                        column: x => x.RootId,
                        principalTable: "Roots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SelectedSuppliers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SelectedSuppliers_NomenclatureId",
                table: "SelectedSuppliers",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedSuppliers_OwnerId",
                table: "SelectedSuppliers",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedSuppliers_RootId",
                table: "SelectedSuppliers",
                column: "RootId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedSuppliers_SupplierId",
                table: "SelectedSuppliers",
                column: "SupplierId");
        }
    }
}
