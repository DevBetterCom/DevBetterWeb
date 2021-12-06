using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations;

public partial class InviteEntityConfig : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropColumn(
        name: "StripeSubscriptionId",
        table: "Invitations");

    migrationBuilder.AlterColumn<string>(
        name: "InviteCode",
        table: "Invitations",
        type: "nvarchar(500)",
        maxLength: 500,
        nullable: false,
        defaultValue: "",
        oldClrType: typeof(string),
        oldType: "nvarchar(max)",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "Email",
        table: "Invitations",
        type: "nvarchar(200)",
        maxLength: 200,
        nullable: false,
        defaultValue: "",
        oldClrType: typeof(string),
        oldType: "nvarchar(max)",
        oldNullable: true);

    migrationBuilder.AddColumn<string>(
        name: "PaymentHandlerSubscriptionId",
        table: "Invitations",
        type: "nvarchar(500)",
        maxLength: 500,
        nullable: false,
        defaultValue: "");
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropColumn(
        name: "PaymentHandlerSubscriptionId",
        table: "Invitations");

    migrationBuilder.AlterColumn<string>(
        name: "InviteCode",
        table: "Invitations",
        type: "nvarchar(max)",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "nvarchar(500)",
        oldMaxLength: 500);

    migrationBuilder.AlterColumn<string>(
        name: "Email",
        table: "Invitations",
        type: "nvarchar(max)",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "nvarchar(200)",
        oldMaxLength: 200);

    migrationBuilder.AddColumn<string>(
        name: "StripeSubscriptionId",
        table: "Invitations",
        type: "nvarchar(max)",
        nullable: true);
  }
}
