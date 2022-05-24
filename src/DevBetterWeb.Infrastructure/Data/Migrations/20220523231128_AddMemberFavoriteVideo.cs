using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBetterWeb.Infrastructure.Data.Migrations;

public partial class AddMemberFavoriteVideo : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey(
			name: "FK_MemberVideoProgress_ArchiveVideos_ArchiveVideoId",
			table: "MemberVideoProgress");

		migrationBuilder.DropIndex(
			name: "IX_MemberVideoProgress_ArchiveVideoId",
			table: "MemberVideoProgress");

		migrationBuilder.DropColumn(
			name: "IsCompleted",
			table: "MemberVideoProgress");

		migrationBuilder.RenameColumn(
			name: "SecondWatched",
			table: "MemberVideoProgress",
			newName: "CurrentDuration");

		migrationBuilder.CreateTable(
			name: "MemberFavoriteArchiveVideos",
			columns: table => new
			{
				MemberId = table.Column<int>(type: "int", nullable: false),
				ArchiveVideoId = table.Column<int>(type: "int", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_MemberFavoriteArchiveVideos", x => new { x.MemberId, x.ArchiveVideoId });
				table.ForeignKey(
					name: "FK_MemberFavoriteArchiveVideos_ArchiveVideos_ArchiveVideoId",
					column: x => x.ArchiveVideoId,
					principalTable: "ArchiveVideos",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				table.ForeignKey(
					name: "FK_MemberFavoriteArchiveVideos_Members_MemberId",
					column: x => x.MemberId,
					principalTable: "Members",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateIndex(
			name: "IX_MemberFavoriteArchiveVideos_ArchiveVideoId",
			table: "MemberFavoriteArchiveVideos",
			column: "ArchiveVideoId");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "MemberFavoriteArchiveVideos");

		migrationBuilder.RenameColumn(
			name: "CurrentDuration",
			table: "MemberVideoProgress",
			newName: "SecondWatched");

		migrationBuilder.AddColumn<bool>(
			name: "IsCompleted",
			table: "MemberVideoProgress",
			type: "bit",
			nullable: false,
			defaultValue: false);

		migrationBuilder.CreateIndex(
			name: "IX_MemberVideoProgress_ArchiveVideoId",
			table: "MemberVideoProgress",
			column: "ArchiveVideoId");

		migrationBuilder.AddForeignKey(
			name: "FK_MemberVideoProgress_ArchiveVideos_ArchiveVideoId",
			table: "MemberVideoProgress",
			column: "ArchiveVideoId",
			principalTable: "ArchiveVideos",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);
	}
}
