using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EpamKse.GameStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddGenreHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenreIds",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParentGenreId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Genres_Genres_ParentGenreId",
                        column: x => x.ParentGenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                column: "GenreIds",
                value: "1,2,4");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2,
                column: "GenreIds",
                value: "11,13");

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name", "ParentGenreId" },
                values: new object[,]
                {
                    { 1, "Strategy", null },
                    { 4, "RPG", null },
                    { 5, "Sports", null },
                    { 11, "Action", null },
                    { 15, "Puzzle & Skill", null },
                    { 2, "RTS", 1 },
                    { 3, "TBS", 1 },
                    { 6, "Races", 5 },
                    { 7, "Rally", 5 },
                    { 8, "Arcade", 5 },
                    { 9, "Formula", 5 },
                    { 10, "Off-road", 5 },
                    { 12, "FPS", 11 },
                    { 13, "TPS", 11 },
                    { 14, "Adventure", 11 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ParentGenreId",
                table: "Genres",
                column: "ParentGenreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropColumn(
                name: "GenreIds",
                table: "Games");
        }
    }
}
