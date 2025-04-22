using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_garage.Migrations
{
    /// <inheritdoc />
    public partial class Addedpostinterest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostImageModel_Posts_PostId1",
                table: "PostImageModel");

            migrationBuilder.DropTable(
                name: "PostCategories");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Posts",
                newName: "InterestId");

            migrationBuilder.AlterColumn<string>(
                name: "PostId1",
                table: "PostImageModel",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_InterestId",
                table: "Posts",
                column: "InterestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostImageModel_Posts_PostId1",
                table: "PostImageModel",
                column: "PostId1",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Interests_InterestId",
                table: "Posts",
                column: "InterestId",
                principalTable: "Interests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostImageModel_Posts_PostId1",
                table: "PostImageModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Interests_InterestId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_InterestId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "InterestId",
                table: "Posts",
                newName: "CategoryId");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Posts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PostId1",
                table: "PostImageModel",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PostCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategories", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PostImageModel_Posts_PostId1",
                table: "PostImageModel",
                column: "PostId1",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
