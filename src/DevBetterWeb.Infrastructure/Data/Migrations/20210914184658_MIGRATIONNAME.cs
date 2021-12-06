using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations;

public partial class MIGRATIONNAME : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.AddColumn<DateTimeOffset>(
        name: "DateUploaded",
        table: "ArchiveVideos",
        type: "datetimeoffset",
        nullable: false,
        defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

    migrationBuilder.AddColumn<string>(
        name: "Description",
        table: "ArchiveVideos",
        type: "nvarchar(max)",
        nullable: true);

    migrationBuilder.AddColumn<int>(
        name: "Duration",
        table: "ArchiveVideos",
        type: "int",
        nullable: false,
        defaultValue: 0);

    migrationBuilder.AddColumn<string>(
        name: "Status",
        table: "ArchiveVideos",
        type: "nvarchar(max)",
        nullable: true);

    migrationBuilder.AddColumn<string>(
        name: "VideoId",
        table: "ArchiveVideos",
        type: "nvarchar(max)",
        nullable: true);

    migrationBuilder.CreateTable(
        name: "MemberVideo",
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

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "MemberVideo");

    migrationBuilder.DropColumn(
        name: "DateUploaded",
        table: "ArchiveVideos");

    migrationBuilder.DropColumn(
        name: "Description",
        table: "ArchiveVideos");

    migrationBuilder.DropColumn(
        name: "Duration",
        table: "ArchiveVideos");

    migrationBuilder.DropColumn(
        name: "Status",
        table: "ArchiveVideos");

    migrationBuilder.DropColumn(
        name: "VideoId",
        table: "ArchiveVideos");
  }
}
