using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSSupplier_Report_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
update suppliers
set suppliers.ReportId = reports.Id
from [dbo].[SSSuppliers] as suppliers
inner join [dbo].[SSDatas] as datas on datas.SupplierId = suppliers.Id
inner join [dbo].[SSVariants] as variants on datas.VariantId = variants.Id
inner join [dbo].[SSReports] as reports on variants.ReportId = reports.Id
");
            migrationBuilder.Sql("delete from [dbo].[SSSuppliers] where ReportId is null ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[SSSuppliers] set ReportId = null");
        }
    }
}
