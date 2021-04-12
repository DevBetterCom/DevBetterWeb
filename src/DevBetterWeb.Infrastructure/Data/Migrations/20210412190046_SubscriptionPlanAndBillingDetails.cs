using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class SubscriptionPlanAndBillingDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details_Message",
                table: "BillingActivities");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanId",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Details_ActionVerbPastTense",
                table: "BillingActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Details_BillingPeriod",
                table: "BillingActivities",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Details_MemberName",
                table: "BillingActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Details_SubscriptionPlanName",
                table: "BillingActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SubscriptionPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Details_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Details_PricePerBillingPeriod = table.Column<decimal>(type: "decimal(18,2)", maxLength: 100, nullable: true),
                    Details_BillingPeriod = table.Column<int>(type: "int", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlan", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionPlan");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "Details_ActionVerbPastTense",
                table: "BillingActivities");

            migrationBuilder.DropColumn(
                name: "Details_BillingPeriod",
                table: "BillingActivities");

            migrationBuilder.DropColumn(
                name: "Details_MemberName",
                table: "BillingActivities");

            migrationBuilder.DropColumn(
                name: "Details_SubscriptionPlanName",
                table: "BillingActivities");

            migrationBuilder.AddColumn<string>(
                name: "Details_Message",
                table: "BillingActivities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
