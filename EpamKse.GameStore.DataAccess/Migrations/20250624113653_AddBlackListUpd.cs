using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpamKse.GameStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBlackListUpd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GameCountryBans",
                columns: new[] { "Id", "Country", "CreatedAt", "GameId" },
                values: new object[] { 1, "UA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "FullName",
                value: "Admin User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GameCountryBans",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "FullName",
                value: "admin");
        }
    }
}
