using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyGroupFinder.Migrations
{
    /// <inheritdoc />
    public partial class request : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingRequests_AspNetUsers_UserId",
                table: "PendingRequests");

            migrationBuilder.DropIndex(
                name: "IX_PendingRequests_UserId",
                table: "PendingRequests");

            migrationBuilder.DropColumn(
                name: "RequestedAt",
                table: "PendingRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PendingRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PendingRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedAt",
                table: "PendingRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_PendingRequests_UserId",
                table: "PendingRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingRequests_AspNetUsers_UserId",
                table: "PendingRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
