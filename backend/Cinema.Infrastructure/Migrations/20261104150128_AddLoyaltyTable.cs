using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cinema.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoyaltyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SeatGapRuleMode",
                table: "CinemaSettings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Strict",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "Strict");

            migrationBuilder.CreateTable(
                name: "LoyaltyPointEntries",
                columns: table => new
                {
                    EntryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    BookingId = table.Column<int>(type: "integer", nullable: true),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    RevokedPoints = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    EarnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyPointEntries", x => x.EntryId);
                    table.CheckConstraint("CK_LoyaltyPointEntries_Points", "\"Points\" >= 0");
                    table.CheckConstraint("CK_LoyaltyPointEntries_Revoked", "\"RevokedPoints\" >= 0 AND \"RevokedPoints\" <= \"Points\"");
                    table.ForeignKey(
                        name: "FK_LoyaltyPointEntries_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoyaltyPointEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyPointEntries_User_EarnedAt",
                table: "LoyaltyPointEntries",
                columns: new[] { "UserId", "EarnedAt" });

            migrationBuilder.CreateIndex(
                name: "UX_LoyaltyPointEntries_Booking",
                table: "LoyaltyPointEntries",
                column: "BookingId",
                unique: true,
                filter: "\"BookingId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoyaltyPointEntries");

            migrationBuilder.AlterColumn<string>(
                name: "SeatGapRuleMode",
                table: "CinemaSettings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "Strict",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Strict");
        }
    }
}
