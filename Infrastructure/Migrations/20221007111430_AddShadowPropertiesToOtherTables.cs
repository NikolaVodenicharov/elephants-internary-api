using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddShadowPropertiesToOtherTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Status");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "MentorSpecialties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "MentorSpecialties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "LearningTopicSpecialities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "LearningTopicSpecialities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CampaignMentors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "CampaignMentors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MentorSpecialties");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MentorSpecialties");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "LearningTopicSpecialities");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "LearningTopicSpecialities");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CampaignMentors");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "CampaignMentors");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Status",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Status",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "StatusId",
                keyValue: 0,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538), new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538) });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "StatusId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538), new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538) });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "StatusId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538), new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538) });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "StatusId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538), new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538) });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "StatusId",
                keyValue: 4,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538), new DateTime(2022, 10, 7, 7, 9, 39, 178, DateTimeKind.Utc).AddTicks(6538) });
        }
    }
}
