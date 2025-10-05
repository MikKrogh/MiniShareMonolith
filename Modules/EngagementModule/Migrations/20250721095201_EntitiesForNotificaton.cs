using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngagementModule.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesForNotificaton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PostComments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ActivityChains",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DateChanged = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityChains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChainListeners",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    AcitivtyChainId = table.Column<string>(type: "text", nullable: false),
                    LastestSyncTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "ActivityChains");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PostComments");
        }
    }
}
