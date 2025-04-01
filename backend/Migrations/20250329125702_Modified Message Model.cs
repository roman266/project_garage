using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_garage.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedMessageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderName",
                table: "Messages",
                newName: "ImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Messages",
                newName: "SenderName");
        }
    }
}
