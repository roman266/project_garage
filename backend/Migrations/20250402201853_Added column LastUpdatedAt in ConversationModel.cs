using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_garage.Migrations
{
    /// <inheritdoc />
    public partial class AddedcolumnLastUpdatedAtinConversationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Conversations",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Conversations");
        }
    }
}
