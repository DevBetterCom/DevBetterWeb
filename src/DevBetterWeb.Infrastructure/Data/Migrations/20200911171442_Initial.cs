using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchiveVideos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    ShowNotes = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                    VideoUrl = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveVideos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    PurchaseUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArchiveVideoId = table.Column<int>(nullable: false),
                    QuestionText = table.Column<string>(maxLength: 500, nullable: true),
                    TimestampSeconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_ArchiveVideos_ArchiveVideoId",
                        column: x => x.ArchiveVideoId,
                        principalTable: "ArchiveVideos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 500, nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: true),
                    LastName = table.Column<string>(maxLength: 100, nullable: true),
                    AboutInfo = table.Column<string>(nullable: true),
                    Address = table.Column<string>(maxLength: 500, nullable: true),
                    PEFriendCode = table.Column<string>(maxLength: 100, nullable: true),
                    PEUsername = table.Column<string>(maxLength: 100, nullable: true),
                    BlogUrl = table.Column<string>(maxLength: 200, nullable: true),
                    GitHubUrl = table.Column<string>(maxLength: 200, nullable: true),
                    LinkedInUrl = table.Column<string>(maxLength: 200, nullable: true),
                    OtherUrl = table.Column<string>(maxLength: 200, nullable: true),
                    TwitchUrl = table.Column<string>(maxLength: 200, nullable: true),
                    YouTubeUrl = table.Column<string>(maxLength: 200, nullable: true),
                    TwitterUrl = table.Column<string>(maxLength: 200, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    BookId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_BookId",
                table: "Members",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_ArchiveVideoId",
                table: "Question",
                column: "ArchiveVideoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "ArchiveVideos");
        }
    }
}
