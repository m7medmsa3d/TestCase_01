using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestCase_01_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TESTCASE",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastExportedAt",
                table: "TESTCASE",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TESTCASE",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TESTCASE");

            migrationBuilder.DropColumn(
                name: "LastExportedAt",
                table: "TESTCASE");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TESTCASE");
        }
    }
}
