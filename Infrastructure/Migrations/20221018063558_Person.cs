using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Person : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });


            migrationBuilder.Sql("INSERT INTO Persons SELECT Id, FirstName, LastName, FirstName + ' ' + LastName, '', PersonalEmail, " +
                "CreatedDate, UpdatedDate FROM Interns");

            migrationBuilder.Sql("INSERT INTO Persons SELECT Id, '', '', DisplayName, Email, '', CreatedDate, UpdatedDate " +
                "FROM Mentors");

            migrationBuilder.CreateTable(
                name: "CampaignPerson",
                columns: table => new
                {
                    CampaignsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignPerson", x => new { x.CampaignsId, x.PersonsId });
                    table.ForeignKey(
                        name: "FK_CampaignPerson_Campaigns_CampaignsId",
                        column: x => x.CampaignsId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignPerson_Persons_PersonsId",
                        column: x => x.PersonsId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonRoles",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonRoles", x => new { x.RoleId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_PersonRoles_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonSpeciality",
                columns: table => new
                {
                    PersonsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonSpeciality", x => new { x.PersonsId, x.SpecialitiesId });
                    table.ForeignKey(
                        name: "FK_PersonSpeciality_Persons_PersonsId",
                        column: x => x.PersonsId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonSpeciality_Specialties_SpecialitiesId",
                        column: x => x.SpecialitiesId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.Sql("INSERT INTO CampaignPerson SELECT CampaignId, MentorId, CreatedDate, UpdatedDate " +
                "FROM CampaignMentors");

            migrationBuilder.Sql("INSERT INTO PersonSpeciality SELECT MentorId, SpecialityId, CreatedDate, UpdatedDate " +
                "FROM MentorSpecialties");

            migrationBuilder.Sql("INSERT INTO PersonRoles SELECT Mentors.Id, Roles.RoleId, Mentors.CreatedDate, Mentors.UpdatedDate " +
                "FROM Mentors " +
                "JOIN Roles on Roles.Name='Mentor'");

            migrationBuilder.Sql("INSERT INTO PersonRoles SELECT Interns.Id, Roles.RoleId, Interns.CreatedDate, Interns.UpdatedDate " +
                "FROM Interns " +
                "JOIN Roles on Roles.Name='Intern'");

            migrationBuilder.DropForeignKey(
                name: "FK_InternCampaigns_Interns_InternId",
                table: "InternCampaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_State_InternCampaigns_InternId_CampaignId",
                table: "State");

            migrationBuilder.DropForeignKey(
                name: "FK_State_Status_StatusId",
                table: "State");

            migrationBuilder.DropTable(
                name: "CampaignMentors");

            migrationBuilder.DropTable(
                name: "Interns");

            migrationBuilder.DropTable(
                name: "MentorSpecialties");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Mentors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_State",
                table: "State");

            migrationBuilder.RenameTable(
                name: "State",
                newName: "States");

            migrationBuilder.RenameColumn(
                name: "InternId",
                table: "InternCampaigns",
                newName: "PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_State_StatusId",
                table: "States",
                newName: "IX_States_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_State_InternId_CampaignId",
                table: "States",
                newName: "IX_States_InternId_CampaignId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_States",
                table: "States",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignPerson_PersonsId",
                table: "CampaignPerson",
                column: "PersonsId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonRoles_PersonId",
                table: "PersonRoles",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonSpeciality_SpecialitiesId",
                table: "PersonSpeciality",
                column: "SpecialitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_InternCampaigns_Persons_PersonId",
                table: "InternCampaigns",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_States_InternCampaigns_InternId_CampaignId",
                table: "States",
                columns: new[] { "InternId", "CampaignId" },
                principalTable: "InternCampaigns",
                principalColumns: new[] { "PersonId", "CampaignId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_States_Status_StatusId",
                table: "States",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternCampaigns_Persons_PersonId",
                table: "InternCampaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_States_InternCampaigns_InternId_CampaignId",
                table: "States");

            migrationBuilder.DropForeignKey(
                name: "FK_States_Status_StatusId",
                table: "States");

            migrationBuilder.DropTable(
                name: "CampaignPerson");

            migrationBuilder.DropTable(
                name: "PersonRoles");

            migrationBuilder.DropTable(
                name: "PersonSpeciality");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_States",
                table: "States");

            migrationBuilder.RenameTable(
                name: "States",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "InternCampaigns",
                newName: "InternId");

            migrationBuilder.RenameIndex(
                name: "IX_States_StatusId",
                table: "State",
                newName: "IX_State_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_States_InternId_CampaignId",
                table: "State",
                newName: "IX_State_InternId_CampaignId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_State",
                table: "State",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Interns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: false),
                    PersonalEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mentors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CampaignMentors",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MentorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignMentors", x => new { x.CampaignId, x.MentorId });
                    table.ForeignKey(
                        name: "CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "MentorId",
                        column: x => x.MentorId,
                        principalTable: "Mentors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MentorSpecialties",
                columns: table => new
                {
                    MentorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MentorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Mentors_MentorId",
                        column: x => x.MentorId,
                        principalTable: "Mentors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignMentors_MentorId",
                table: "CampaignMentors",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorSpecialties_SpecialityId",
                table: "MentorSpecialties",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_MentorId",
                table: "Users",
                column: "MentorId",
                unique: true,
                filter: "[MentorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_InternCampaigns_Interns_InternId",
                table: "InternCampaigns",
                column: "InternId",
                principalTable: "Interns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_State_InternCampaigns_InternId_CampaignId",
                table: "State",
                columns: new[] { "InternId", "CampaignId" },
                principalTable: "InternCampaigns",
                principalColumns: new[] { "InternId", "CampaignId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_State_Status_StatusId",
                table: "State",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
