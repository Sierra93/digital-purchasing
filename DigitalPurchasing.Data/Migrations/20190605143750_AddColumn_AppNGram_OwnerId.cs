using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddColumn_AppNGram_OwnerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "AppNGrams",
                nullable: true);

            migrationBuilder.Sql(@"
update dbo.AppNGrams set OwnerId = (
    select top (1) n.OwnerId
    from
        dbo.Nomenclatures n
        join dbo.NomenclatureComparisonDatas ncd on n.Id = ncd.NomenclatureId
    where ncd.Id = dbo.AppNGrams.NomenclatureComparisonDataId
)
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "AppNGrams");
        }
    }
}
