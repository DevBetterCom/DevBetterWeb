using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class SubscriptionRenamedToMemberSubscription : Migration
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

            migrationBuilder.AlterColumn<decimal>(
                name: "Details_PricePerBillingPeriod",
                table: "SubscriptionPlan",
                type: "decimal(18,2)",
                maxLength: 100,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Details_Name",
                table: "SubscriptionPlan",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Details_BillingPeriod",
                table: "SubscriptionPlan",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100,
                oldNullable: true);

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

            migrationBuilder.AlterColumn<decimal>(
                name: "Details_PricePerBillingPeriod",
                table: "SubscriptionPlan",
                type: "decimal(18,2)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Details_Name",
                table: "SubscriptionPlan",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "Details_BillingPeriod",
                table: "SubscriptionPlan",
                type: "int",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100);

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
