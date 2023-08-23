using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopNews.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitHomePC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "38459126-950a-4f96-8dcb-a9eaa46f768c", null, "Admin", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "38459126-950a-4f96-8dcb-a9eaa46f768c");
        }
    }
}
