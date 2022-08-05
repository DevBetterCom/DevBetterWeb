using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class changes_fix_02082022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_ArchiveVideos_ArchiveVideoId",
                table: "Question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Question",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_ArchiveVideoId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "ShowNotes",
                table: "ArchiveVideos");

            migrationBuilder.RenameTable(
                name: "Question",
                newName: "Questions");

            migrationBuilder.RenameColumn(
                name: "TimestampSeconds",
                table: "Questions",
                newName: "Votes");

            migrationBuilder.AddColumn<int>(
                name: "BookCategoryId",
                table: "Books",
                type: "int",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "Questions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

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
                name: "BookCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCategories", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "QuestionVote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionVote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionVote_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionVote_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookCategoryId",
                table: "Books",
                column: "BookCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CoachingSessionId",
                table: "Questions",
                column: "CoachingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_MemberId",
                table: "Questions",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVote_MemberId",
                table: "QuestionVote",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVote_QuestionId",
                table: "QuestionVote",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookCategories_BookCategoryId",
                table: "Books",
                column: "BookCategoryId",
                principalTable: "BookCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_CoachingSessions_CoachingSessionId",
                table: "Questions",
                column: "CoachingSessionId",
                principalTable: "CoachingSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Books_BookCategories_BookCategoryId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_CoachingSessions_CoachingSessionId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Members_MemberId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "BookCategories");

            migrationBuilder.DropTable(
                name: "CoachingSessions");

            migrationBuilder.DropTable(
                name: "QuestionVote");

            migrationBuilder.DropIndex(
                name: "IX_Books_BookCategoryId",
                table: "Books");

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
                name: "BookCategoryId",
                table: "Books");

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

            migrationBuilder.AddColumn<string>(
                name: "ShowNotes",
                table: "ArchiveVideos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "Question",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

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

            migrationBuilder.CreateIndex(
                name: "IX_Question_ArchiveVideoId",
                table: "Question",
                column: "ArchiveVideoId");

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
