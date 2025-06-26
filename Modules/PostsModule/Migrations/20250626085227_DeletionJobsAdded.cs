using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostsModule.Migrations
{
    /// <inheritdoc />
    public partial class DeletionJobsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeletionJobs",
                schema: "PostModule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ImagesDeletionCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    ThumbnailRemovedCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    PostDeletedEventPublished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletionJobs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletionJobs",
                schema: "PostModule");
        }
    }
}
