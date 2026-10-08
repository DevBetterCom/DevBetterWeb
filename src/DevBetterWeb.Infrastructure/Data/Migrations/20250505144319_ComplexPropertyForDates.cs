using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ComplexPropertyForDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberSubscriptionDates");

            migrationBuilder.AddColumn<DateTime>(
                name: "Dates_EndDate",
                table: "MemberSubscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Dates_StartDate",
                table: "MemberSubscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dates_EndDate",
                table: "MemberSubscriptions");

            migrationBuilder.DropColumn(
                name: "Dates_StartDate",
                table: "MemberSubscriptions");

            migrationBuilder.CreateTable(
                name: "MemberSubscriptionDates",
                columns: table => new
                {
                    MemberSubscriptionId = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberSubscriptionDates", x => x.MemberSubscriptionId);
                    table.ForeignKey(
                        name: "FK_MemberSubscriptionDates_MemberSubscriptions_MemberSubscriptionId",
                        column: x => x.MemberSubscriptionId,
                        principalTable: "MemberSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
