using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewsOzetleyici.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Summaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    SummaryText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SummaryLength = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AiModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConfidenceScore = table.Column<float>(type: "real", nullable: true),
                    ProcessingTimeMs = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Summaries_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "#3f51b5", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1275), "Genel haberler", "Genel", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1278) },
                    { 2, "#4caf50", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1893), "Teknoloji haberleri", "Teknoloji", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1893) },
                    { 3, "#ff9800", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1896), "Spor haberleri", "Spor", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1896) },
                    { 4, "#f44336", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1897), "Ekonomi haberleri", "Ekonomi", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1898) },
                    { 5, "#9c27b0", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1899), "Sağlık haberleri", "Sağlık", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1899) },
                    { 6, "#00bcd4", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1900), "Bilim haberleri", "Bilim", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1901) },
                    { 7, "#795548", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1902), "Eğitim haberleri", "Eğitim", new DateTime(2025, 6, 23, 19, 28, 19, 839, DateTimeKind.Utc).AddTicks(1903) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_News_CategoryId",
                table: "News",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_News_Url",
                table: "News",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_NewsId",
                table: "Summaries",
                column: "NewsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Summaries");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
