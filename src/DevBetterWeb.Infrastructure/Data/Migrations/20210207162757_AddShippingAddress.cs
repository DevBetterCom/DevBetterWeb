using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class AddShippingAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CityLocation_Latitude",
                table: "Members",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CityLocation_Longitude",
                table: "Members",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_City",
                table: "Members",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Country",
                table: "Members",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_PostalCode",
                table: "Members",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_State",
                table: "Members",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Street",
                table: "Members",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityLocation_Latitude",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CityLocation_Longitude",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_City",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Country",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_PostalCode",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_State",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Street",
                table: "Members");
        }
    }
}
