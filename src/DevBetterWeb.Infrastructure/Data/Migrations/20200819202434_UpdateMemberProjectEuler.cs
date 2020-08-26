using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class UpdateMemberProjectEuler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PEFriendCode",
                table: "Members",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PEUsername",
                table: "Members",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PEFriendCode",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "PEUsername",
                table: "Members");
        }
    }
}
