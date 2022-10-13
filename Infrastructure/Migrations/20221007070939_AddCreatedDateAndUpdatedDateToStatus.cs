using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddCreatedDateAndUpdatedDateToStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Status");
        }
    }
}
