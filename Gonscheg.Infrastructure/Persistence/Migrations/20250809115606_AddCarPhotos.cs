using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gonscheg.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCarPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarPhotoPath",
                table: "ChatUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarPhotoPath",
                table: "ChatUsers");
        }
    }
}
