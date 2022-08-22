using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class book_uploader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberWhoUploadId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_MemberWhoUploadId",
                table: "Books",
                column: "MemberWhoUploadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Members_MemberWhoUploadId",
                table: "Books",
                column: "MemberWhoUploadId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Members_MemberWhoUploadId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_MemberWhoUploadId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "MemberWhoUploadId",
                table: "Books");
        }
    }
}
