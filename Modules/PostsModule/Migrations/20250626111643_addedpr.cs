using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostsModule.Migrations
{
    /// <inheritdoc />
    public partial class addedpr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PostDataDeletionCompleted",
                schema: "PostModule",
                table: "DeletionJobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostDataDeletionCompleted",
                schema: "PostModule",
                table: "DeletionJobs");
        }
    }
}
