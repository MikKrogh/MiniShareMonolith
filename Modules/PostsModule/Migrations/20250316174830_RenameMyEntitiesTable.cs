using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostsModule.Migrations
{
    /// <inheritdoc />
    public partial class RenameMyEntitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PostModule");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "StoreSchema",
                newName: "Users",
                newSchema: "PostModule");

            migrationBuilder.RenameTable(
                name: "Posts",
                schema: "StoreSchema",
                newName: "Posts",
                newSchema: "PostModule");

            migrationBuilder.RenameTable(
                name: "Image",
                schema: "StoreSchema",
                newName: "Image",
                newSchema: "PostModule");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "StoreSchema");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "PostModule",
                newName: "Users",
                newSchema: "StoreSchema");

            migrationBuilder.RenameTable(
                name: "Posts",
                schema: "PostModule",
                newName: "Posts",
                newSchema: "StoreSchema");

            migrationBuilder.RenameTable(
                name: "Image",
                schema: "PostModule",
                newName: "Image",
                newSchema: "StoreSchema");
        }
    }
}
