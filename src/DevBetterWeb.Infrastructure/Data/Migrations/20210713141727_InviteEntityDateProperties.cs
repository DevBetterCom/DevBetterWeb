using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class InviteEntityDateProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Invitations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfLastAdminPing",
                table: "Invitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfUserPing",
                table: "Invitations",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "DateOfLastAdminPing",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "DateOfUserPing",
                table: "Invitations");
        }
    }
}
