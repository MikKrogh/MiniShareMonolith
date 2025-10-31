using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngagementModule.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityChains",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<string>(type: "text", nullable: false),
                    DateChanged = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityChains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostComments",
                columns: table => new
                {
                    CommentId = table.Column<string>(type: "text", nullable: false),
                    ParentCommentId = table.Column<string>(type: "text", nullable: true),
                    PostId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostComments", x => x.CommentId);
                });

            migrationBuilder.CreateTable(
                name: "PostLikes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostLikes", x => new { x.UserId, x.PostId });
                });

            migrationBuilder.CreateTable(
                name: "UserSync",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LastSyncTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSync", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ChainListeners",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    AcitivtyChainId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainListeners", x => new { x.UserId, x.AcitivtyChainId });
                    table.ForeignKey(
                        name: "FK_ChainListeners_ActivityChains_AcitivtyChainId",
                        column: x => x.AcitivtyChainId,
                        principalTable: "ActivityChains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChainListeners_AcitivtyChainId",
                table: "ChainListeners",
                column: "AcitivtyChainId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChainListeners");

            migrationBuilder.DropTable(
                name: "PostComments");

            migrationBuilder.DropTable(
                name: "PostLikes");

            migrationBuilder.DropTable(
                name: "UserSync");

            migrationBuilder.DropTable(
                name: "ActivityChains");
        }
    }
}
