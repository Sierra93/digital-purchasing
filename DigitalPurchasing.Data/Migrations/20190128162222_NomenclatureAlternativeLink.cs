using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class NomenclatureAlternativeLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [NomenclatureAlternatives]");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "ClientType",
                table: "NomenclatureAlternatives");

            migrationBuilder.CreateTable(
                name: "NomenclatureAlternativeLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AlternativeId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: true),
                    SupplierId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureAlternativeLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomenclatureAlternativeLinks_NomenclatureAlternatives_AlternativeId",
                        column: x => x.AlternativeId,
                        principalTable: "NomenclatureAlternatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NomenclatureAlternativeLinks_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NomenclatureAlternativeLinks_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternativeLinks_AlternativeId",
                table: "NomenclatureAlternativeLinks",
                column: "AlternativeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternativeLinks_CustomerId",
                table: "NomenclatureAlternativeLinks",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternativeLinks_SupplierId",
                table: "NomenclatureAlternativeLinks",
                column: "SupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NomenclatureAlternativeLinks");

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "NomenclatureAlternatives",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientType",
                table: "NomenclatureAlternatives",
                nullable: false,
                defaultValue: 0);
        }
    }
}
