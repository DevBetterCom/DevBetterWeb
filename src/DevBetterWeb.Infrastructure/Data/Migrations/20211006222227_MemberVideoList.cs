using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class MemberVideoList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnimatedThumbnailUri",
                table: "ArchiveVideos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "ArchiveVideos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimatedThumbnailUri",
                table: "ArchiveVideos");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "ArchiveVideos");
        }
    }
}
