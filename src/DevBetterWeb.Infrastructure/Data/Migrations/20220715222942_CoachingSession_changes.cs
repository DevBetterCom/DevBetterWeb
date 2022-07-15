using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class CoachingSession_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_ArchiveVideos_ArchiveVideoId",
                table: "Question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Question",
                table: "Question");

            migrationBuilder.RenameTable(
                name: "Question",
                newName: "Questions");

            migrationBuilder.RenameColumn(
                name: "TimestampSeconds",
                table: "Questions",
                newName: "Votes");

            migrationBuilder.RenameIndex(
                name: "IX_Question_ArchiveVideoId",
                table: "Questions",
                newName: "IX_Questions_ArchiveVideoId");

            migrationBuilder.AlterColumn<int>(
                name: "ArchiveVideoId",
                table: "Questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CoachingSessionId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Questions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CoachingSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachingSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CoachingSessionId",
                table: "Questions",
                column: "CoachingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_MemberId",
                table: "Questions",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ArchiveVideos_ArchiveVideoId",
                table: "Questions",
                column: "ArchiveVideoId",
                principalTable: "ArchiveVideos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_CoachingSessions_CoachingSessionId",
                table: "Questions",
                column: "CoachingSessionId",
                principalTable: "CoachingSessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Members_MemberId",
                table: "Questions",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ArchiveVideos_ArchiveVideoId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_CoachingSessions_CoachingSessionId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Members_MemberId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "CoachingSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CoachingSessionId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_MemberId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CoachingSessionId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Questions");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "Question");

            migrationBuilder.RenameColumn(
                name: "Votes",
                table: "Question",
                newName: "TimestampSeconds");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_ArchiveVideoId",
                table: "Question",
                newName: "IX_Question_ArchiveVideoId");

            migrationBuilder.AlterColumn<int>(
                name: "ArchiveVideoId",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Question",
                table: "Question",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_ArchiveVideos_ArchiveVideoId",
                table: "Question",
                column: "ArchiveVideoId",
                principalTable: "ArchiveVideos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
