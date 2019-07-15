using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSSupplierItem_NewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OfferInvoiceData",
                table: "SSSupplierItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UomStr",
                table: "SSSupplierItems",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE
    ssItem
SET
    ssItem.UomStr = soi.RawUomStr,
    ssItem.OfferInvoiceData = so.InvoiceData
FROM
    dbo.SSSupplierItems AS ssItem
    INNER JOIN dbo.SupplierOfferItems AS soi ON ssItem.InternalId = soi.Id
    INNER JOIN dbo.SupplierOffers AS so ON so.Id = soi.SupplierOfferId
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferInvoiceData",
                table: "SSSupplierItems");

            migrationBuilder.DropColumn(
                name: "UomStr",
                table: "SSSupplierItems");
        }
    }
}
