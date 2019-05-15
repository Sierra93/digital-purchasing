using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Files_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailAttachments_ReceivedEmails_ReceivedEmailId",
                table: "EmailAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailAttachments",
                table: "EmailAttachments");

            migrationBuilder.RenameTable(
                name: "EmailAttachments",
                newName: "Files");

            migrationBuilder.RenameIndex(
                name: "IX_EmailAttachments_ReceivedEmailId",
                table: "Files",
                newName: "IX_Files_ReceivedEmailId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceivedEmailId",
                table: "Files",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Files",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_ReceivedEmails_ReceivedEmailId",
                table: "Files",
                column: "ReceivedEmailId",
                principalTable: "ReceivedEmails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("update dbo.Files set Discriminator = 'EmailAttachments'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_ReceivedEmails_ReceivedEmailId",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "EmailAttachments");

            migrationBuilder.RenameIndex(
                name: "IX_Files_ReceivedEmailId",
                table: "EmailAttachments",
                newName: "IX_EmailAttachments_ReceivedEmailId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceivedEmailId",
                table: "EmailAttachments",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailAttachments",
                table: "EmailAttachments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailAttachments_ReceivedEmails_ReceivedEmailId",
                table: "EmailAttachments",
                column: "ReceivedEmailId",
                principalTable: "ReceivedEmails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
