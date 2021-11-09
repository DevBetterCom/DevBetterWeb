using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations;

public partial class BillingActivity : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "BillingActivities",
        columns: table => new
        {
          Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
          MemberId = table.Column<int>(type: "int", maxLength: 500, nullable: false),
          Date = table.Column<DateTime>(type: "datetime2", nullable: false),
          Details_Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
          Details_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_BillingActivities", x => x.Id);
          table.ForeignKey(
                      name: "FK_BillingActivities_Members_MemberId",
                      column: x => x.MemberId,
                      principalTable: "Members",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_BillingActivities_MemberId",
        table: "BillingActivities",
        column: "MemberId");
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "BillingActivities");
  }
}
