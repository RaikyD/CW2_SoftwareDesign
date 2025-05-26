using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileAnalysisService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilesData",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    hash = table.Column<int>(type: "integer", nullable: false),
                    words = table.Column<int>(type: "integer", nullable: false),
                    paragraphs = table.Column<int>(type: "integer", nullable: false),
                    symbols = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesData", x => x.FileId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilesData");
        }
    }
}
