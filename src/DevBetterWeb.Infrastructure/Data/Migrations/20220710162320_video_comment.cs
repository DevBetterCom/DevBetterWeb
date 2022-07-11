using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class video_comment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberVideoProgress");

            migrationBuilder.CreateTable(
                name: "VideoComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    ParentCommentId = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoComments_ArchiveVideos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "ArchiveVideos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoComments_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoComments_VideoComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "VideoComments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoComments_MemberId",
                table: "VideoComments",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoComments_ParentCommentId",
                table: "VideoComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoComments_VideoId",
                table: "VideoComments",
                column: "VideoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoComments");

            migrationBuilder.CreateTable(
                name: "MemberVideoProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArchiveVideoId = table.Column<int>(type: "int", nullable: false),
                    CurrentDuration = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberVideoProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberVideoProgress_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberVideoProgress_MemberId",
                table: "MemberVideoProgress",
                column: "MemberId");
        }
    }
}
