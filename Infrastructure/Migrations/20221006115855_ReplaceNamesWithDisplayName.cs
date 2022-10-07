using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ReplaceNamesWithDisplayName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Mentors");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Mentors");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Mentors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Mentors");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Mentors",
                type: "nvarchar(125)",
                maxLength: 125,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Mentors",
                type: "nvarchar(125)",
                maxLength: 125,
                nullable: false,
                defaultValue: "");
        }
    }
}
