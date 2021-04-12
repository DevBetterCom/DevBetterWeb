using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class SubscriptionRenameToMemberSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionDates_Subscriptions_SubscriptionId",
                table: "SubscriptionDates");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "SubscriptionDates",
                newName: "MemberSubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionDates_Subscriptions_MemberSubscriptionId",
                table: "SubscriptionDates",
                column: "MemberSubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionDates_Subscriptions_MemberSubscriptionId",
                table: "SubscriptionDates");

            migrationBuilder.RenameColumn(
                name: "MemberSubscriptionId",
                table: "SubscriptionDates",
                newName: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionDates_Subscriptions_SubscriptionId",
                table: "SubscriptionDates",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
