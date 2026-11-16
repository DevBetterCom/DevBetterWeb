using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBetterWeb.Modules.Testimonials.Infrastructure.Data.Migrations;

/// <inheritdoc />
internal partial class AddTestimonials : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.EnsureSchema(
				name: "testimonials");

		migrationBuilder.CreateTable(
				name: "Testimonials",
				schema: "testimonials",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
								.Annotation("SqlServer:Identity", "1, 1"),
					MemberId = table.Column<int>(type: "int", nullable: false),
					Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
					AuthorDisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					AuthorTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					AuthorOrganization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					AuthorPhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
					Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
					ReviewerFeedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
					CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					SubmittedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Testimonials", x => x.Id);
				});

		migrationBuilder.CreateIndex(
				name: "IX_Testimonials_MemberId",
				schema: "testimonials",
				table: "Testimonials",
				column: "MemberId");

		migrationBuilder.CreateIndex(
				name: "IX_Testimonials_Status_ReviewedAt",
				schema: "testimonials",
				table: "Testimonials",
				columns: new[] { "Status", "ReviewedAt" });
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
				name: "Testimonials",
				schema: "testimonials");
	}
}
