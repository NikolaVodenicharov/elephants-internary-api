using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class FixRelationBetweenMentorsAndSpecialties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mentors_Specialties_SpecialityId",
                table: "Mentors");

            migrationBuilder.DropIndex(
                name: "IX_Mentors_SpecialityId",
                table: "Mentors");

            migrationBuilder.DropColumn(
                name: "SpecialityId",
                table: "Mentors");

            migrationBuilder.CreateTable(
                name: "MentorSpecialties",
                columns: table => new
                {
                    MentorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorSpecialties", x => new { x.MentorId, x.SpecialityId });
                    table.ForeignKey(
                        name: "FK_MentorId",
                        column: x => x.MentorId,
                        principalTable: "Mentors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MentorSpecialties_SpecialityId",
                table: "MentorSpecialties",
                column: "SpecialityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MentorSpecialties");

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialityId",
                table: "Mentors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Mentors_SpecialityId",
                table: "Mentors",
                column: "SpecialityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mentors_Specialties_SpecialityId",
                table: "Mentors",
                column: "SpecialityId",
                principalTable: "Specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
