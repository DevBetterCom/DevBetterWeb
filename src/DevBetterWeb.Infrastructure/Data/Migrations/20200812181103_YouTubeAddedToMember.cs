using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class YouTubeAddedToMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GithubUrl",
                table: "Members",
                newName: "GitHubUrl");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Members",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeUrl",
                table: "Members",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeUrl",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "GitHubUrl",
                table: "Members",
                newName: "GithubUrl");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Members",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500);
        }
    }
}
