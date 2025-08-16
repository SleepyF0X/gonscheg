using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gonscheg.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisterDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDate",
                table: "ChatUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 8, 10, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisterDate",
                table: "ChatUsers");
        }
    }
}
