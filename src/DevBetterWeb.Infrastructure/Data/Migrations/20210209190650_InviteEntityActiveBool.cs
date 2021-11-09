using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations;

public partial class InviteEntityActiveBool : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.AddColumn<bool>(
        name: "Active",
        table: "Invitations",
        type: "bit",
        nullable: false,
        defaultValue: false);
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropColumn(
        name: "Active",
        table: "Invitations");
  }
}
