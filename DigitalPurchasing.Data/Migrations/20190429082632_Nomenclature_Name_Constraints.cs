using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Nomenclature_Name_Constraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Quick fix: rename existed dupes
            migrationBuilder.Sql(@"
 update dbo.Nomenclatures
 set name = name + ' ' + convert(nvarchar(50), newid())
 where name in (select name from dbo.Nomenclatures group by ownerid, name having count(*) > 1)

 update dbo.NomenclatureAlternatives
 set name = name + ' ' + convert(nvarchar(50), newid())
 where name in (select name from dbo.NomenclatureAlternatives group by ownerid, name having count(*) > 1)
");

            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_OwnerId",
                table: "Nomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_OwnerId",
                table: "NomenclatureAlternatives");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Nomenclatures",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NomenclatureAlternatives",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_OwnerId_Name",
                table: "Nomenclatures",
                columns: new[] { "OwnerId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_OwnerId_Name",
                table: "NomenclatureAlternatives",
                columns: new[] { "OwnerId", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_OwnerId_Name",
                table: "Nomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_OwnerId_Name",
                table: "NomenclatureAlternatives");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Nomenclatures",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NomenclatureAlternatives",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_OwnerId",
                table: "Nomenclatures",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_OwnerId",
                table: "NomenclatureAlternatives",
                column: "OwnerId");
        }
    }
}
