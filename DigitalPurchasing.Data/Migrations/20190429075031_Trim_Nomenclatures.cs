using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Trim_Nomenclatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
UPDATE dbo.Nomenclatures SET Name = Trim(Name), Code = Trim(Code)
UPDATE dbo.NomenclatureAlternatives SET Name = Trim(Name), Code = Trim(Code)
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
