using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierContactPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    SupplierId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierContactPersons_Companies_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierContactPersons_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationRequestEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    RequestId = table.Column<Guid>(nullable: false),
                    ContactPersonId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationRequestEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationRequestEmails_SupplierContactPersons_ContactPersonId",
                        column: x => x.ContactPersonId,
                        principalTable: "SupplierContactPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationRequestEmails_QuotationRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "QuotationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequestEmails_ContactPersonId",
                table: "QuotationRequestEmails",
                column: "ContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequestEmails_RequestId",
                table: "QuotationRequestEmails",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierContactPersons_OwnerId",
                table: "SupplierContactPersons",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierContactPersons_SupplierId",
                table: "SupplierContactPersons",
                column: "SupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuotationRequestEmails");

            migrationBuilder.DropTable(
                name: "SupplierContactPersons");
        }
    }
}
