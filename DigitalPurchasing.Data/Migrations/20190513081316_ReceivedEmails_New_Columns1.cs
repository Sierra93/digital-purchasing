using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ReceivedEmails_New_Columns1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromEmail",
                table: "ReceivedEmails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmailAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    Bytes = table.Column<byte[]>(nullable: false),
                    ContentType = table.Column<string>(nullable: false),
                    ReceivedEmailId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailAttachments_ReceivedEmails_ReceivedEmailId",
                        column: x => x.ReceivedEmailId,
                        principalTable: "ReceivedEmails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailAttachments_ReceivedEmailId",
                table: "EmailAttachments",
                column: "ReceivedEmailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailAttachments");

            migrationBuilder.DropColumn(
                name: "FromEmail",
                table: "ReceivedEmails");
        }
    }
}
