using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_garage.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPostModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostImages_AspNetUsers_UserModelId",
                table: "PostImages");

            migrationBuilder.DropForeignKey(
                name: "FK_PostImages_Posts_PostId",
                table: "PostImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostImages",
                table: "PostImages");

            migrationBuilder.DropIndex(
                name: "IX_PostImages_PostId",
                table: "PostImages");

            migrationBuilder.RenameTable(
                name: "PostImages",
                newName: "PostImageModel");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Posts",
                newName: "ImageUrl");

            migrationBuilder.RenameIndex(
                name: "IX_PostImages_UserModelId",
                table: "PostImageModel",
                newName: "IX_PostImageModel_UserModelId");

            migrationBuilder.AddColumn<string>(
                name: "PostId1",
                table: "PostImageModel",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostImageModel",
                table: "PostImageModel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostImageModel_PostId1",
                table: "PostImageModel",
                column: "PostId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PostImageModel_AspNetUsers_UserModelId",
                table: "PostImageModel",
                column: "UserModelId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostImageModel_Posts_PostId1",
                table: "PostImageModel",
                column: "PostId1",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostImageModel_AspNetUsers_UserModelId",
                table: "PostImageModel");

            migrationBuilder.DropForeignKey(
                name: "FK_PostImageModel_Posts_PostId1",
                table: "PostImageModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostImageModel",
                table: "PostImageModel");

            migrationBuilder.DropIndex(
                name: "IX_PostImageModel_PostId1",
                table: "PostImageModel");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "PostImageModel");

            migrationBuilder.RenameTable(
                name: "PostImageModel",
                newName: "PostImages");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Posts",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_PostImageModel_UserModelId",
                table: "PostImages",
                newName: "IX_PostImages_UserModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostImages",
                table: "PostImages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostImages_PostId",
                table: "PostImages",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostImages_AspNetUsers_UserModelId",
                table: "PostImages",
                column: "UserModelId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostImages_Posts_PostId",
                table: "PostImages",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
