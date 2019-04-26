using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Supplier_PublicId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PublicId",
                table: "Suppliers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SupplierCounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    CurrentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierCounters_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCounters_OwnerId",
                table: "SupplierCounters",
                column: "OwnerId",
                unique: true);

            migrationBuilder.Sql(@"
;WITH sRowNum (Id, rowNumber)  
AS  
(  
	select s.Id, ROW_NUMBER() OVER (PARTITION BY OwnerId ORDER BY CreatedOn DESC) as rowNumber
	from dbo.Suppliers s
)  
UPDATE
    s1
SET
    s1.PublicId = s2.rowNumber
FROM
    dbo.Suppliers AS s1
    INNER JOIN sRowNum AS s2 ON s1.Id = s2.Id

INSERT INTO dbo.SupplierCounters (Id, CreatedOn, OwnerId, CurrentId)
SELECT newid(), getutcdate(), s.OwnerId, max(s.PublicId)
FROM dbo.Suppliers s GROUP BY s.OwnerId
");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_OwnerId_PublicId",
                table: "Suppliers",
                columns: new[] { "OwnerId", "PublicId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierCounters");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_OwnerId_PublicId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Suppliers");
        }
    }
}
