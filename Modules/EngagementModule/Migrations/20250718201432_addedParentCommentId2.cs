using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngagementModule.Migrations
{
    /// <inheritdoc />
    public partial class addedParentCommentId2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentCommentId",
                table: "PostComments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "PostComments");
        }
    }
}
