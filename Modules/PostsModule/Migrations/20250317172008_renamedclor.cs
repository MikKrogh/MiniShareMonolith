using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostsModule.Migrations
{
    /// <inheritdoc />
    public partial class renamedclor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecondaryColour",
                schema: "PostModule",
                table: "Posts",
                newName: "SecondaryColor");

            migrationBuilder.RenameColumn(
                name: "PrimaryColour",
                schema: "PostModule",
                table: "Posts",
                newName: "PrimaryColor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecondaryColor",
                schema: "PostModule",
                table: "Posts",
                newName: "SecondaryColour");

            migrationBuilder.RenameColumn(
                name: "PrimaryColor",
                schema: "PostModule",
                table: "Posts",
                newName: "PrimaryColour");
        }
    }
}
