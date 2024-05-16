using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Baetoti.Infrastructure.Migrations
{
    public partial class _20231215 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MarkAsDeleted",
                schema: "baetoti",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FreelanceCertID",
                schema: "baetoti",
                table: "Driver",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FreelanceCertPic",
                schema: "baetoti",
                table: "Driver",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FreelanceExpireDate",
                schema: "baetoti",
                table: "Driver",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FreelanceIssueDate",
                schema: "baetoti",
                table: "Driver",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SignaturePic",
                schema: "baetoti",
                table: "Driver",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkAsDeleted",
                schema: "baetoti",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FreelanceCertID",
                schema: "baetoti",
                table: "Driver");

            migrationBuilder.DropColumn(
                name: "FreelanceCertPic",
                schema: "baetoti",
                table: "Driver");

            migrationBuilder.DropColumn(
                name: "FreelanceExpireDate",
                schema: "baetoti",
                table: "Driver");

            migrationBuilder.DropColumn(
                name: "FreelanceIssueDate",
                schema: "baetoti",
                table: "Driver");

            migrationBuilder.DropColumn(
                name: "SignaturePic",
                schema: "baetoti",
                table: "Driver");
        }
    }
}
