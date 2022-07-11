using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class LAST_MIGRATION_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberVideoProgress_Members_MemberId",
                table: "MemberVideoProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberVideoProgress",
                table: "MemberVideoProgress");

            migrationBuilder.RenameTable(
                name: "MemberVideoProgress",
                newName: "MembersVideosProgress");

            migrationBuilder.RenameIndex(
                name: "IX_MemberVideoProgress_MemberId",
                table: "MembersVideosProgress",
                newName: "IX_MembersVideosProgress_MemberId");

            migrationBuilder.AddColumn<string>(
                name: "VideoWatchedStatus",
                table: "MembersVideosProgress",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembersVideosProgress",
                table: "MembersVideosProgress",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "VideoComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    ParentCommentId = table.Column<int>(type: "int", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                name: "IX_MembersVideosProgress_ArchiveVideoId",
                table: "MembersVideosProgress",
                column: "ArchiveVideoId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_MembersVideosProgress_ArchiveVideos_ArchiveVideoId",
                table: "MembersVideosProgress",
                column: "ArchiveVideoId",
                principalTable: "ArchiveVideos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MembersVideosProgress_Members_MemberId",
                table: "MembersVideosProgress",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembersVideosProgress_ArchiveVideos_ArchiveVideoId",
                table: "MembersVideosProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_MembersVideosProgress_Members_MemberId",
                table: "MembersVideosProgress");

            migrationBuilder.DropTable(
                name: "VideoComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MembersVideosProgress",
                table: "MembersVideosProgress");

            migrationBuilder.DropIndex(
                name: "IX_MembersVideosProgress_ArchiveVideoId",
                table: "MembersVideosProgress");

            migrationBuilder.DropColumn(
                name: "VideoWatchedStatus",
                table: "MembersVideosProgress");

            migrationBuilder.RenameTable(
                name: "MembersVideosProgress",
                newName: "MemberVideoProgress");

            migrationBuilder.RenameIndex(
                name: "IX_MembersVideosProgress_MemberId",
                table: "MemberVideoProgress",
                newName: "IX_MemberVideoProgress_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberVideoProgress",
                table: "MemberVideoProgress",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberVideoProgress_Members_MemberId",
                table: "MemberVideoProgress",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
