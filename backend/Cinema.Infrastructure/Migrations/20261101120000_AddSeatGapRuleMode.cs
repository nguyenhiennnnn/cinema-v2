using Cinema.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261101120000_AddSeatGapRuleMode")]
    public partial class AddSeatGapRuleMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatGapRuleMode",
                table: "CinemaSettings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Strict");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatGapRuleMode",
                table: "CinemaSettings");
        }
    }
}
