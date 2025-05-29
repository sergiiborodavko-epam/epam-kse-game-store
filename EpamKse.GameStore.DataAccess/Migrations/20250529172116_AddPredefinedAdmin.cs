using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpamKse.GameStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPredefinedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "PasswordHash", "Role", "UserName" },
                values: new object[] { 1, new DateTime(2025, 5, 29, 20, 21, 16, 45, DateTimeKind.Local).AddTicks(3825), "admin@example.com", "admin", "$argon2id$v=19$m=65536,t=3,p=1$hIWcROP/j0uU/PceT+/jHw$Kn1RHnAoDdMitEPzaT43//MwsEDJMwAjEPr8liXCHrM", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
