using Microsoft.EntityFrameworkCore.Migrations;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    public partial class AddMemberEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: true),
                    LastName = table.Column<string>(maxLength: 100, nullable: true),
                    Address = table.Column<string>(maxLength: 500, nullable: true),
                    LinkedInUrl = table.Column<string>(maxLength: 200, nullable: true),
                    TwitterUrl = table.Column<string>(maxLength: 200, nullable: true),
                    GithubUrl = table.Column<string>(maxLength: 200, nullable: true),
                    BlogUrl = table.Column<string>(maxLength: 200, nullable: true),
                    TwitchUrl = table.Column<string>(maxLength: 200, nullable: true),
                    OtherUrl = table.Column<string>(maxLength: 200, nullable: true),
                    AboutInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
