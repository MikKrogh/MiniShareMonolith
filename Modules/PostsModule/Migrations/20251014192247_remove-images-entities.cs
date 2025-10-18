using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostsModule.Migrations
{
    /// <inheritdoc />
    public partial class removeimagesentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Image",
                schema: "PostModule");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Image",
                schema: "PostModule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "PostModule",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_PostId",
                schema: "PostModule",
                table: "Image",
                column: "PostId");
        }
    }
}
