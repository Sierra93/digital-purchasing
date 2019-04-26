using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class NewColumn_Customer_PublicId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_OwnerId",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "PublicId",
                table: "Customers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CustomerCounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    CurrentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCounters_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCounters_OwnerId",
                table: "CustomerCounters",
                column: "OwnerId",
                unique: true);

            migrationBuilder.Sql(@"
;WITH customerRowNum (Id, rowNumber)  
AS  
(  
	select c.Id, ROW_NUMBER() OVER (PARTITION BY OwnerId order by CreatedOn) as rowNumber
	from dbo.Customers c
)  
UPDATE
    c1
SET
    c1.PublicId = c2.rowNumber
FROM
    dbo.Customers AS c1
    INNER JOIN customerRowNum AS c2 ON c1.Id = c2.Id

INSERT INTO dbo.CustomerCounters (Id, CreatedOn, OwnerId, CurrentId)
SELECT newid(), getutcdate(), c.OwnerId, max(c.PublicId)
FROM dbo.Customers c GROUP BY c.OwnerId
");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_OwnerId_PublicId",
                table: "Customers",
                columns: new[] { "OwnerId", "PublicId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerCounters");

            migrationBuilder.DropIndex(
                name: "IX_Customers_OwnerId_PublicId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_OwnerId",
                table: "Customers",
                column: "OwnerId");
        }
    }
}
