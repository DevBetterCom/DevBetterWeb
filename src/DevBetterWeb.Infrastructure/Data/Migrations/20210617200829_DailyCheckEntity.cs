using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations;

public partial class DailyCheckEntity : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "DailyChecks",
        columns: table => new
        {
          Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
          Date = table.Column<DateTime>(type: "datetime2", nullable: false),
          TasksCompleted = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_DailyChecks", x => x.Id);
        });
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "DailyChecks");
  }
}
