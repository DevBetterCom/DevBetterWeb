using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class VideoProgress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberVideo");

            migrationBuilder.CreateTable(
                name: "MemberVideoProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    ArchiveVideoId = table.Column<int>(type: "int", nullable: false),
                    SecondWatched = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberVideoProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberVideoProgress_ArchiveVideos_ArchiveVideoId",
                        column: x => x.ArchiveVideoId,
                        principalTable: "ArchiveVideos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberVideoProgress_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberVideoProgress_ArchiveVideoId",
                table: "MemberVideoProgress",
                column: "ArchiveVideoId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberVideoProgress_MemberId",
                table: "MemberVideoProgress",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberVideoProgress");

            migrationBuilder.CreateTable(
                name: "MemberVideo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArchiveVideoId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    SecondWatched = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberVideo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberVideo_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberVideo_MemberId",
                table: "MemberVideo",
                column: "MemberId");
        }
    }
}
