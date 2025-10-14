using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngagementModule.Migrations
{
    /// <inheritdoc />
    public partial class syncUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastestSyncTime",
                table: "ChainListeners");

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "ActivityChains",
                type: "text",
                nullable: false,
                defaultValue: "");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSync");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "ActivityChains");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastestSyncTime",
                table: "ChainListeners",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
