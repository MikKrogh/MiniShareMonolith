using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostsModule.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PostModule");

            migrationBuilder.CreateTable(
                name: "DeletionJobs",
                schema: "PostModule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ImagesDeletionCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    PostDataDeletionCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    ThumbnailRemovedCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    PostDeletedEventPublished = table.Column<bool>(type: "boolean", nullable: false),
                    FailedAttempts = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletionJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "PostModule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "PostModule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Faction = table.Column<string>(type: "text", nullable: false),
                    PrimaryColor = table.Column<string>(type: "text", nullable: false),
                    SecondaryColor = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalSchema: "PostModule",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatorId",
                schema: "PostModule",
                table: "Posts",
                column: "CreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletionJobs",
                schema: "PostModule");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "PostModule");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "PostModule");
        }
    }
}
