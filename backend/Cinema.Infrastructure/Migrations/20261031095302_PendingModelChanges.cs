using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CheckInOpenMinutes",
                table: "CinemaSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HoldDurationMinutes",
                table: "CinemaSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CinemaSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CancelRefundPercent", "CheckInOpenMinutes", "HoldDurationMinutes" },
                values: new object[] { 0m, 30, 15 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInOpenMinutes",
                table: "CinemaSettings");

            migrationBuilder.DropColumn(
                name: "HoldDurationMinutes",
                table: "CinemaSettings");

            migrationBuilder.UpdateData(
                table: "CinemaSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "CancelRefundPercent",
                value: 100m);
        }
    }
}
