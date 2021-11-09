using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations;

public partial class MapCoordinates : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.AddColumn<decimal>(
        name: "CityLatitude",
        table: "Members",
        type: "decimal(18,2)",
        nullable: true);

    migrationBuilder.AddColumn<decimal>(
        name: "CityLongitude",
        table: "Members",
        type: "decimal(18,2)",
        nullable: true);
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropColumn(
        name: "CityLatitude",
        table: "Members");

    migrationBuilder.DropColumn(
        name: "CityLongitude",
        table: "Members");
  }
}
