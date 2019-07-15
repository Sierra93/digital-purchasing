using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ReceivedEmail_Root : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RootId",
                table: "ReceivedEmails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedEmails_RootId",
                table: "ReceivedEmails",
                column: "RootId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedEmails_Roots_RootId",
                table: "ReceivedEmails",
                column: "RootId",
                principalTable: "Roots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedEmails_Roots_RootId",
                table: "ReceivedEmails");

            migrationBuilder.DropIndex(
                name: "IX_ReceivedEmails_RootId",
                table: "ReceivedEmails");

            migrationBuilder.DropColumn(
                name: "RootId",
                table: "ReceivedEmails");
        }
    }
}
