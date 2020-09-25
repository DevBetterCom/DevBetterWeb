using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class properManyToManyNow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Books_BookId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Members_MemberId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_BookId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_MemberId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "BookMember",
                columns: table => new
                {
                    BooksReadId = table.Column<int>(type: "int", nullable: false),
                    MembersWhoHaveReadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookMember", x => new { x.BooksReadId, x.MembersWhoHaveReadId });
                    table.ForeignKey(
                        name: "FK_BookMember_Books_BooksReadId",
                        column: x => x.BooksReadId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookMember_Members_MembersWhoHaveReadId",
                        column: x => x.MembersWhoHaveReadId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookMember_MembersWhoHaveReadId",
                table: "BookMember",
                column: "MembersWhoHaveReadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookMember");

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookId",
                table: "Books",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_MemberId",
                table: "Books",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Books_BookId",
                table: "Books",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Members_MemberId",
                table: "Books",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
