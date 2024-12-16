using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyGroupFinder.Migrations
{
    /// <inheritdoc />
    public partial class Pending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "PendingRequests");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedAt",
                table: "PendingRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestedAt",
                table: "PendingRequests");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PendingRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
