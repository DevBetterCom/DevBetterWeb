using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class SubscriptionFullRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionDates_Subscriptions_MemberSubscriptionId",
                table: "SubscriptionDates");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Members_MemberId",
                table: "Subscriptions");

            migrationBuilder.DropTable(
                name: "SubscriptionPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionDates",
                table: "SubscriptionDates");

            migrationBuilder.RenameTable(
                name: "Subscriptions",
                newName: "MemberSubscriptions");

            migrationBuilder.RenameTable(
                name: "SubscriptionDates",
                newName: "MemberSubscriptionDates");

            migrationBuilder.RenameColumn(
                name: "SubscriptionPlanId",
                table: "MemberSubscriptions",
                newName: "MemberSubscriptionPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_MemberId",
                table: "MemberSubscriptions",
                newName: "IX_MemberSubscriptions_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberSubscriptions",
                table: "MemberSubscriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberSubscriptionDates",
                table: "MemberSubscriptionDates",
                column: "MemberSubscriptionId");

            migrationBuilder.CreateTable(
                name: "MemberSubscriptionPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Details_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details_PricePerBillingPeriod = table.Column<decimal>(type: "decimal(18,2)", maxLength: 100, nullable: false),
                    Details_BillingPeriod = table.Column<int>(type: "int", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberSubscriptionPlan", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MemberSubscriptionDates_MemberSubscriptions_MemberSubscriptionId",
                table: "MemberSubscriptionDates",
                column: "MemberSubscriptionId",
                principalTable: "MemberSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberSubscriptions_Members_MemberId",
                table: "MemberSubscriptions",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberSubscriptionDates_MemberSubscriptions_MemberSubscriptionId",
                table: "MemberSubscriptionDates");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberSubscriptions_Members_MemberId",
                table: "MemberSubscriptions");

            migrationBuilder.DropTable(
                name: "MemberSubscriptionPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberSubscriptions",
                table: "MemberSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberSubscriptionDates",
                table: "MemberSubscriptionDates");

            migrationBuilder.RenameTable(
                name: "MemberSubscriptions",
                newName: "Subscriptions");

            migrationBuilder.RenameTable(
                name: "MemberSubscriptionDates",
                newName: "SubscriptionDates");

            migrationBuilder.RenameColumn(
                name: "MemberSubscriptionPlanId",
                table: "Subscriptions",
                newName: "SubscriptionPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberSubscriptions_MemberId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionDates",
                table: "SubscriptionDates",
                column: "MemberSubscriptionId");

            migrationBuilder.CreateTable(
                name: "SubscriptionPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Details_BillingPeriod = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    Details_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details_PricePerBillingPeriod = table.Column<decimal>(type: "decimal(18,2)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlan", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionDates_Subscriptions_MemberSubscriptionId",
                table: "SubscriptionDates",
                column: "MemberSubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Members_MemberId",
                table: "Subscriptions",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
