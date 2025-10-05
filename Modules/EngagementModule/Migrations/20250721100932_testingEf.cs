using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngagementModule.Migrations
{
    /// <inheritdoc />
    public partial class testingEf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TmpClass",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DateChanged = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmpClass", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TmpClass");
        }
    }
}
