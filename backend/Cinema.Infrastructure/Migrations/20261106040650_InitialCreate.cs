using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cinema.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AvatarUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ActorId);
                });

            migrationBuilder.CreateTable(
                name: "AgeRatings",
                columns: table => new
                {
                    AgeRatingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeRatings", x => x.AgeRatingId);
                });

            migrationBuilder.CreateTable(
                name: "CinemaSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OpeningTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    ClosingTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PointsPerAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 10000m),
                    CancelRefundPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    CancelDeadlineHours = table.Column<int>(type: "integer", nullable: false),
                    StopSellingMinutesBefore = table.Column<int>(type: "integer", nullable: false),
                    HoldDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    CheckInOpenMinutes = table.Column<int>(type: "integer", nullable: false),
                    SeatGapRuleMode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Strict"),
                    RoomBufferMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 15),
                    MaxSeatsPerBooking = table.Column<int>(type: "integer", nullable: false, defaultValue: 10),
                    MaxComboQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 10),
                    LoyaltyWindowMonths = table.Column<int>(type: "integer", nullable: false, defaultValue: 12),
                    MovieEndGraceDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ShowtimeLookaheadDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 7),
                    SchedulerStepMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    SchedulerMinSameMovieGapMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 60),
                    SchedulerHeatLookbackDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 7),
                    SchedulerHeatSalesWeight = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.69999999999999996),
                    SchedulerHeatRatingWeight = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.29999999999999999),
                    SchedulerMaxRating = table.Column<double>(type: "double precision", nullable: false, defaultValue: 5.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CinemaSettings", x => x.Id);
                    table.CheckConstraint("CK_CinemaSettings_Id", "\"Id\" = 1");
                });

            migrationBuilder.CreateTable(
                name: "Combos",
                columns: table => new
                {
                    ComboId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combos", x => x.ComboId);
                    table.CheckConstraint("CK_Combos_Price", "\"Price\" >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    DirectorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AvatarUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.DirectorId);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    GenreId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.GenreId);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "MembershipTiers",
                columns: table => new
                {
                    TierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MinPoints = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DiscountPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipTiers", x => x.TierId);
                    table.CheckConstraint("CK_MembershipTiers_Discount", "\"DiscountPercent\" BETWEEN 0 AND 100");
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "ScreenTypes",
                columns: table => new
                {
                    ScreenTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenTypes", x => x.ScreenTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SeatTypes",
                columns: table => new
                {
                    SeatTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PriceMultiplier = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 1.0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatTypes", x => x.SeatTypeId);
                    table.CheckConstraint("CK_SeatTypes_PriceMultiplier", "\"PriceMultiplier\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OriginalTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DirectorId = table.Column<int>(type: "integer", nullable: true),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LanguageId = table.Column<int>(type: "integer", nullable: true),
                    Subtitle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AgeRatingId = table.Column<int>(type: "integer", nullable: true),
                    PosterUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TrailerUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "ComingSoon"),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.MovieId);
                    table.CheckConstraint("CK_Movies_AverageRating", "\"AverageRating\" BETWEEN 0 AND 5");
                    table.CheckConstraint("CK_Movies_Duration", "\"DurationMinutes\" > 0");
                    table.CheckConstraint("CK_Movies_Status", "\"Status\" IN ('ComingSoon','NowShowing','Ended')");
                    table.ForeignKey(
                        name: "FK_Movies_AgeRatings_AgeRatingId",
                        column: x => x.AgeRatingId,
                        principalTable: "AgeRatings",
                        principalColumn: "AgeRatingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Movies_Directors_DirectorId",
                        column: x => x.DirectorId,
                        principalTable: "Directors",
                        principalColumn: "DirectorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Movies_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Avatar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    GoogleId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.CheckConstraint("CK_Users_Gender", "\"Gender\" IN ('Male','Female','Other')");
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CinemaRooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ScreenTypeId = table.Column<int>(type: "integer", nullable: true),
                    TotalSeats = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Rows = table.Column<int>(type: "integer", nullable: false),
                    Cols = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CinemaRooms", x => x.RoomId);
                    table.CheckConstraint("CK_CinemaRooms_TotalSeats", "\"TotalSeats\" >= 0");
                    table.ForeignKey(
                        name: "FK_CinemaRooms_ScreenTypes_ScreenTypeId",
                        column: x => x.ScreenTypeId,
                        principalTable: "ScreenTypes",
                        principalColumn: "ScreenTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovieCast",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    ActorId = table.Column<int>(type: "integer", nullable: false),
                    CharacterName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCast", x => new { x.MovieId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_MovieCast_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "ActorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovieCast_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenres",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    GenreId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenres", x => new { x.MovieId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_MovieGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovieGenres_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerProfiles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TierId = table.Column<int>(type: "integer", nullable: true),
                    TotalSpent = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_CustomerProfiles_MembershipTiers_TierId",
                        column: x => x.TierId,
                        principalTable: "MembershipTiers",
                        principalColumn: "TierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PositionId = table.Column<int>(type: "integer", nullable: true),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    RevokedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReplacedByTokenId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_RefreshTokens_ReplacedByTokenId",
                        column: x => x.ReplacedByTokenId,
                        principalTable: "RefreshTokens",
                        principalColumn: "RefreshTokenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.CheckConstraint("CK_Reviews_Rating", "\"Rating\" BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Reviews_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserVerificationTokens",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Purpose = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVerificationTokens", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_UserVerificationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    SeatId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    SeatRow = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    SeatNumber = table.Column<int>(type: "integer", nullable: false),
                    SeatTypeId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsAisle = table.Column<bool>(type: "boolean", nullable: false),
                    CoupleId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.SeatId);
                    table.CheckConstraint("CK_Seats_SeatNumber", "\"SeatNumber\" > 0");
                    table.ForeignKey(
                        name: "FK_Seats_CinemaRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "CinemaRooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Seats_SeatTypes_SeatTypeId",
                        column: x => x.SeatTypeId,
                        principalTable: "SeatTypes",
                        principalColumn: "SeatTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Showtimes",
                columns: table => new
                {
                    ShowtimeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Scheduled"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Showtimes", x => x.ShowtimeId);
                    table.CheckConstraint("CK_Showtime_Time", "\"EndTime\" > \"StartTime\"");
                    table.CheckConstraint("CK_Showtimes_BasePrice", "\"BasePrice\" >= 0");
                    table.CheckConstraint("CK_Showtimes_Status", "\"Status\" IN ('Scheduled','Cancelled','Completed')");
                    table.ForeignKey(
                        name: "FK_Showtimes_CinemaRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "CinemaRooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Showtimes_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    ShowtimeId = table.Column<int>(type: "integer", nullable: false),
                    BookingCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Online"),
                    CreatedByEmployeeId = table.Column<int>(type: "integer", nullable: true),
                    CustomerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    FinalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.CheckConstraint("CK_Booking_Channel_Employee", "(\"Channel\" = 'Counter' AND \"CreatedByEmployeeId\" IS NOT NULL) OR (\"Channel\" = 'Online' AND \"CreatedByEmployeeId\" IS NULL)");
                    table.CheckConstraint("CK_Booking_Customer_Identity", "\"UserId\" IS NOT NULL OR \"CustomerName\" IS NOT NULL");
                    table.CheckConstraint("CK_Bookings_Channel", "\"Channel\" IN ('Online','Counter')");
                    table.CheckConstraint("CK_Bookings_DiscountAmount", "\"DiscountAmount\" >= 0");
                    table.CheckConstraint("CK_Bookings_FinalAmount", "\"FinalAmount\" >= 0");
                    table.CheckConstraint("CK_Bookings_Status", "\"Status\" IN ('Pending','Confirmed','Cancelled','Expired')");
                    table.CheckConstraint("CK_Bookings_TotalAmount", "\"TotalAmount\" >= 0");
                    table.ForeignKey(
                        name: "FK_Bookings_Employees_CreatedByEmployeeId",
                        column: x => x.CreatedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Showtimes_ShowtimeId",
                        column: x => x.ShowtimeId,
                        principalTable: "Showtimes",
                        principalColumn: "ShowtimeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingCombos",
                columns: table => new
                {
                    BookingComboId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookingId = table.Column<int>(type: "integer", nullable: false),
                    ComboId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCombos", x => x.BookingComboId);
                    table.CheckConstraint("CK_BookingCombos_Quantity", "\"Quantity\" > 0");
                    table.CheckConstraint("CK_BookingCombos_UnitPrice", "\"UnitPrice\" >= 0");
                    table.ForeignKey(
                        name: "FK_BookingCombos_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingCombos_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "ComboId",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookingId = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    GatewayTransactionId = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    GatewayResponseCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    PaidAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    RefundTransactionId = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    RefundedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AmountReceived = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ChangeGiven = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.CheckConstraint("CK_Payments_Amount", "\"Amount\" >= 0");
                    table.CheckConstraint("CK_Payments_Method", "\"PaymentMethod\" IN ('VNPay','Momo','Cash','Stripe')");
                    table.CheckConstraint("CK_Payments_Status", "\"Status\" IN ('Pending','Success','Failed','Refunded')");
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShowtimeSeats",
                columns: table => new
                {
                    ShowtimeSeatId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShowtimeId = table.Column<int>(type: "integer", nullable: false),
                    SeatId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Available"),
                    HeldByUserId = table.Column<int>(type: "integer", nullable: true),
                    HeldByBookingId = table.Column<int>(type: "integer", nullable: true),
                    HeldUntil = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowtimeSeats", x => x.ShowtimeSeatId);
                    table.CheckConstraint("CK_ShowtimeSeats_Price", "\"Price\" >= 0");
                    table.CheckConstraint("CK_ShowtimeSeats_Status", "\"Status\" IN ('Available','Held','Booked')");
                    table.ForeignKey(
                        name: "FK_ShowtimeSeats_Bookings_HeldByBookingId",
                        column: x => x.HeldByBookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ShowtimeSeats_Seats_SeatId",
                        column: x => x.SeatId,
                        principalTable: "Seats",
                        principalColumn: "SeatId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShowtimeSeats_Showtimes_ShowtimeId",
                        column: x => x.ShowtimeId,
                        principalTable: "Showtimes",
                        principalColumn: "ShowtimeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowtimeSeats_Users_HeldByUserId",
                        column: x => x.HeldByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingTickets",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookingId = table.Column<int>(type: "integer", nullable: false),
                    ShowtimeSeatId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    QrCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsCheckedIn = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CheckedInAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTickets", x => x.TicketId);
                    table.CheckConstraint("CK_BookingTickets_Price", "\"Price\" >= 0");
                    table.ForeignKey(
                        name: "FK_BookingTickets_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingTickets_ShowtimeSeats_ShowtimeSeatId",
                        column: x => x.ShowtimeSeatId,
                        principalTable: "ShowtimeSeats",
                        principalColumn: "ShowtimeSeatId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "ActorId", "AvatarUrl", "FullName" },
                values: new object[,]
                {
                    { 1, "https://ui-avatars.com/api/?name=Cillian+Murphy&background=34495e&color=fff&size=500&bold=true&font-size=0.4", "Cillian Murphy" },
                    { 2, "https://ui-avatars.com/api/?name=Robert+Downey+Jr&background=8b0000&color=fff&size=500&bold=true&font-size=0.4", "Robert Downey Jr." },
                    { 3, "https://ui-avatars.com/api/?name=Timothee+Chalamet&background=c19a49&color=fff&size=500&bold=true&font-size=0.4", "Timothée Chalamet" },
                    { 4, "https://ui-avatars.com/api/?name=Zendaya&background=9b59b6&color=fff&size=500&bold=true&font-size=0.4", "Zendaya" },
                    { 5, "https://ui-avatars.com/api/?name=Tran+Thanh&background=b83b5e&color=fff&size=500&bold=true&font-size=0.4", "Trấn Thành" },
                    { 6, "https://ui-avatars.com/api/?name=Tuan+Tran&background=2980b9&color=fff&size=500&bold=true&font-size=0.4", "Tuấn Trần" },
                    { 7, "https://ui-avatars.com/api/?name=Uyen+An&background=e67e22&color=fff&size=500&bold=true&font-size=0.4", "Uyển Ân" },
                    { 8, "https://ui-avatars.com/api/?name=Phuong+Anh+Dao&background=d63384&color=fff&size=500&bold=true&font-size=0.4", "Phương Anh Đào" },
                    { 9, "https://ui-avatars.com/api/?name=Song+Kang+Ho&background=2d3436&color=fff&size=500&bold=true&font-size=0.4", "Song Kang-ho" },
                    { 10, "https://ui-avatars.com/api/?name=Choi+Woo+Shik&background=16a085&color=fff&size=500&bold=true&font-size=0.4", "Choi Woo-shik" },
                    { 11, "https://ui-avatars.com/api/?name=Sam+Worthington&background=0f4c75&color=fff&size=500&bold=true&font-size=0.4", "Sam Worthington" },
                    { 12, "https://ui-avatars.com/api/?name=Zoe+Saldana&background=1abc9c&color=fff&size=500&bold=true&font-size=0.4", "Zoe Saldana" },
                    { 13, "https://ui-avatars.com/api/?name=Ryan+Reynolds&background=8b0000&color=fff&size=500&bold=true&font-size=0.4", "Ryan Reynolds" },
                    { 14, "https://ui-avatars.com/api/?name=Hugh+Jackman&background=34495e&color=fff&size=500&bold=true&font-size=0.4", "Hugh Jackman" },
                    { 15, "https://ui-avatars.com/api/?name=Thai+Hoa&background=d35400&color=fff&size=500&bold=true&font-size=0.4", "Thái Hòa" },
                    { 16, "https://ui-avatars.com/api/?name=Kaity+Nguyen&background=e84393&color=fff&size=500&bold=true&font-size=0.4", "Kaity Nguyễn" },
                    { 17, "https://ui-avatars.com/api/?name=Ninh+Duong+Lan+Ngoc&background=6c5b7b&color=fff&size=500&bold=true&font-size=0.4", "Ninh Dương Lan Ngọc" },
                    { 18, "https://ui-avatars.com/api/?name=Quach+Ngoc+Ngoan&background=1b1b2f&color=fff&size=500&bold=true&font-size=0.4", "Quách Ngọc Ngoan" },
                    { 19, "https://ui-avatars.com/api/?name=Avin+Lu&background=3498db&color=fff&size=500&bold=true&font-size=0.4", "Avin Lu" },
                    { 20, "https://ui-avatars.com/api/?name=Hoang+Ha&background=e67e22&color=fff&size=500&bold=true&font-size=0.4", "Hoàng Hà" },
                    { 21, "https://ui-avatars.com/api/?name=Amy+Poehler&background=f39c12&color=fff&size=500&bold=true&font-size=0.4", "Amy Poehler" },
                    { 22, "https://ui-avatars.com/api/?name=Maya+Hawke&background=9b59b6&color=fff&size=500&bold=true&font-size=0.4", "Maya Hawke" },
                    { 23, "https://ui-avatars.com/api/?name=Matthew+McConaughey&background=2c3e50&color=fff&size=500&bold=true&font-size=0.4", "Matthew McConaughey" },
                    { 24, "https://ui-avatars.com/api/?name=Anne+Hathaway&background=8e44ad&color=fff&size=500&bold=true&font-size=0.4", "Anne Hathaway" },
                    { 25, "https://ui-avatars.com/api/?name=Tran+Nghia&background=16a085&color=fff&size=500&bold=true&font-size=0.4", "Trần Nghĩa" },
                    { 26, "https://ui-avatars.com/api/?name=Truc+Anh&background=e91e63&color=fff&size=500&bold=true&font-size=0.4", "Trúc Anh" },
                    { 27, "https://ui-avatars.com/api/?name=Choi+Min+Sik&background=34495e&color=fff&size=500&bold=true&font-size=0.4", "Choi Min-sik" },
                    { 28, "https://ui-avatars.com/api/?name=Kim+Go+Eun&background=9c27b0&color=fff&size=500&bold=true&font-size=0.4", "Kim Go-eun" },
                    { 29, "https://ui-avatars.com/api/?name=Lee+Do+Hyun&background=3f51b5&color=fff&size=500&bold=true&font-size=0.4", "Lee Do-hyun" },
                    { 30, "https://ui-avatars.com/api/?name=Shameik+Moore&background=d32f2f&color=fff&size=500&bold=true&font-size=0.4", "Shameik Moore" },
                    { 31, "https://ui-avatars.com/api/?name=Hailee+Steinfeld&background=0288d1&color=fff&size=500&bold=true&font-size=0.4", "Hailee Steinfeld" },
                    { 32, "https://ui-avatars.com/api/?name=Chris+Evans&background=1976d2&color=fff&size=500&bold=true&font-size=0.4", "Chris Evans" },
                    { 33, "https://ui-avatars.com/api/?name=Scarlett+Johansson&background=c2185b&color=fff&size=500&bold=true&font-size=0.4", "Scarlett Johansson" },
                    { 34, "https://ui-avatars.com/api/?name=Jack+Black&background=f57c00&color=fff&size=500&bold=true&font-size=0.4", "Jack Black" },
                    { 35, "https://ui-avatars.com/api/?name=Song+Luan&background=00796b&color=fff&size=500&bold=true&font-size=0.4", "Song Luân" },
                    { 36, "https://ui-avatars.com/api/?name=Le+Giang&background=7b1fa2&color=fff&size=500&bold=true&font-size=0.4", "Lê Giang" },
                    { 37, "https://ui-avatars.com/api/?name=Nanoka+Hara&background=0097a7&color=fff&size=500&bold=true&font-size=0.4", "Nanoka Hara" },
                    { 38, "https://ui-avatars.com/api/?name=Hokuto+Matsumura&background=455a64&color=fff&size=500&bold=true&font-size=0.4", "Hokuto Matsumura" }
                });

            migrationBuilder.InsertData(
                table: "AgeRatings",
                columns: new[] { "AgeRatingId", "Code", "Description" },
                values: new object[,]
                {
                    { 1, "P", "Phổ biến, không hạn chế đối tượng xem" },
                    { 2, "K", "Phổ biến, khuyến cáo trẻ dưới 13 tuổi xem cùng cha mẹ" },
                    { 3, "T13", "Cấm phổ biến với khán giả dưới 13 tuổi" },
                    { 4, "T16", "Cấm phổ biến với khán giả dưới 16 tuổi" },
                    { 5, "T18", "Cấm phổ biến với khán giả dưới 18 tuổi" },
                    { 6, "C", "Không được phép phổ biến" }
                });

            migrationBuilder.InsertData(
                table: "CinemaSettings",
                columns: new[] { "Id", "Address", "CancelDeadlineHours", "CheckInOpenMinutes", "ClosingTime", "Email", "HoldDurationMinutes", "LogoUrl", "LoyaltyWindowMonths", "MaxComboQuantity", "MaxSeatsPerBooking", "MovieEndGraceDays", "Name", "OpeningTime", "Phone", "PointsPerAmount", "RoomBufferMinutes", "SchedulerHeatLookbackDays", "SchedulerHeatRatingWeight", "SchedulerHeatSalesWeight", "SchedulerMaxRating", "SchedulerMinSameMovieGapMinutes", "SchedulerStepMinutes", "ShowtimeLookaheadDays", "StopSellingMinutesBefore", "TaxCode", "UpdatedAt" },
                values: new object[] { 1, "Đang cập nhật", 2, 30, new TimeOnly(23, 59, 0), null, 15, null, 12, 10, 10, 1, "Cinema", new TimeOnly(8, 0, 0), null, 10000m, 15, 7, 0.29999999999999999, 0.69999999999999996, 5.0, 60, 5, 7, 0, null, null });

            migrationBuilder.InsertData(
                table: "Combos",
                columns: new[] { "ComboId", "Description", "ImageUrl", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "1 Bắp ngọt (vừa) + 1 Coca-Cola 22oz", "https://images.unsplash.com/photo-1585647347483-22b66260dfff?w=500", true, "Solo Combo", 65000m },
                    { 2, "1 Bắp ngọt (lớn) + 2 Coca-Cola 22oz", "https://images.unsplash.com/photo-1572177812156-58036aae439c?w=500", true, "Double Combo", 95000m },
                    { 3, "1 Bắp phô mai (lớn) + 2 Coca-Cola 32oz + 1 Snack khoai tây", "https://images.unsplash.com/photo-1512149673953-1e251807ec7c?w=500", true, "VIP Cheese Combo", 129000m },
                    { 4, "2 Bắp ngọt (lớn) + 4 Coca-Cola 22oz + 2 Snack ngũ vị", "https://images.unsplash.com/photo-1505686994434-e3cc5abf1330?w=500", true, "Family Party Combo", 189000m },
                    { 5, "1 Bắp Caramel đặc biệt (lớn) + 2 Nước ngọt Sprite/Fanta 22oz", "https://images.unsplash.com/photo-1578849278619-e73505e9610f?w=500", true, "Caramel Sweet Combo", 109000m }
                });

            migrationBuilder.InsertData(
                table: "Directors",
                columns: new[] { "DirectorId", "AvatarUrl", "FullName" },
                values: new object[,]
                {
                    { 1, "https://ui-avatars.com/api/?name=Christopher+Nolan&background=1a1a2e&color=fff&size=500&bold=true&font-size=0.4", "Christopher Nolan" },
                    { 2, "https://ui-avatars.com/api/?name=Denis+Villeneuve&background=c19a49&color=fff&size=500&bold=true&font-size=0.4", "Denis Villeneuve" },
                    { 3, "https://ui-avatars.com/api/?name=Tran+Thanh&background=b83b5e&color=fff&size=500&bold=true&font-size=0.4", "Trấn Thành" },
                    { 4, "https://ui-avatars.com/api/?name=Victor+Vu&background=1b1b2f&color=fff&size=500&bold=true&font-size=0.4", "Victor Vũ" },
                    { 5, "https://ui-avatars.com/api/?name=Ly+Hai&background=d35400&color=fff&size=500&bold=true&font-size=0.4", "Lý Hải" },
                    { 6, "https://ui-avatars.com/api/?name=James+Cameron&background=0f4c75&color=fff&size=500&bold=true&font-size=0.4", "James Cameron" },
                    { 7, "https://ui-avatars.com/api/?name=Bong+Joon+Ho&background=2d3436&color=fff&size=500&bold=true&font-size=0.4", "Bong Joon-ho" },
                    { 8, "https://ui-avatars.com/api/?name=Phan+Gia+Nhat+Linh&background=6c5b7b&color=fff&size=500&bold=true&font-size=0.4", "Phan Gia Nhật Linh" },
                    { 9, "https://ui-avatars.com/api/?name=Shawn+Levy&background=8b0000&color=fff&size=500&bold=true&font-size=0.4", "Shawn Levy" },
                    { 10, "https://ui-avatars.com/api/?name=Kelsey+Mann&background=f39c12&color=fff&size=500&bold=true&font-size=0.4", "Kelsey Mann" },
                    { 11, "https://ui-avatars.com/api/?name=Russo+Brothers&background=34495e&color=fff&size=500&bold=true&font-size=0.4", "Anthony & Joe Russo" },
                    { 12, "https://ui-avatars.com/api/?name=Jang+Jae+Hyun&background=2c3e50&color=fff&size=500&bold=true&font-size=0.4", "Jang Jae-hyun" },
                    { 13, "https://ui-avatars.com/api/?name=Joaquim+Dos+Santos&background=e74c3c&color=fff&size=500&bold=true&font-size=0.4", "Joaquim Dos Santos" },
                    { 14, "https://ui-avatars.com/api/?name=Mike+Mitchell&background=27ae60&color=fff&size=500&bold=true&font-size=0.4", "Mike Mitchell" },
                    { 15, "https://ui-avatars.com/api/?name=Makoto+Shinkai&background=16a085&color=fff&size=500&bold=true&font-size=0.4", "Makoto Shinkai" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "GenreId", "Name" },
                values: new object[,]
                {
                    { 1, "Hành động" },
                    { 2, "Hài" },
                    { 3, "Chính kịch" },
                    { 4, "Kinh dị" },
                    { 5, "Khoa học viễn tưởng" },
                    { 6, "Hoạt hình" },
                    { 7, "Lãng mạn" },
                    { 8, "Tâm lý" },
                    { 9, "Phiêu lưu" },
                    { 10, "Bí ẩn" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "LanguageId", "Name" },
                values: new object[,]
                {
                    { 1, "Tiếng Việt" },
                    { 2, "Tiếng Anh" },
                    { 3, "Tiếng Hàn" },
                    { 4, "Tiếng Nhật" }
                });

            migrationBuilder.InsertData(
                table: "MembershipTiers",
                columns: new[] { "TierId", "TierName" },
                values: new object[] { 1, "Thành viên" });

            migrationBuilder.InsertData(
                table: "MembershipTiers",
                columns: new[] { "TierId", "DiscountPercent", "MinPoints", "TierName" },
                values: new object[,]
                {
                    { 2, 5m, 500, "Bạc" },
                    { 3, 10m, 2000, "Vàng" },
                    { 4, 15m, 5000, "Kim Cương" }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "PositionId", "Name" },
                values: new object[,]
                {
                    { 1, "Quản lý rạp" },
                    { 2, "Trưởng ca" },
                    { 3, "Nhân viên bán vé" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "Customer" },
                    { 2, "Admin" },
                    { 3, "Employee" }
                });

            migrationBuilder.InsertData(
                table: "ScreenTypes",
                columns: new[] { "ScreenTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "2D" },
                    { 2, "3D" },
                    { 3, "IMAX" }
                });

            migrationBuilder.InsertData(
                table: "SeatTypes",
                columns: new[] { "SeatTypeId", "Name", "PriceMultiplier" },
                values: new object[,]
                {
                    { 1, "Standard", 1.0m },
                    { 2, "VIP", 1.5m },
                    { 3, "Couple", 2.0m }
                });

            migrationBuilder.InsertData(
                table: "CinemaRooms",
                columns: new[] { "RoomId", "Cols", "IsActive", "RoomName", "Rows", "ScreenTypeId", "TotalSeats" },
                values: new object[,]
                {
                    { 1, 10, true, "Cinema Hall 01", 5, 1, 50 },
                    { 2, 10, true, "Cinema Hall 02", 5, 1, 50 },
                    { 3, 10, true, "3D Experience Hall 03", 4, 2, 40 },
                    { 4, 10, true, "IMAX Grand Theatre 04", 6, 3, 60 }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "AgeRatingId", "AverageRating", "CreatedAt", "Description", "DirectorId", "DurationMinutes", "LanguageId", "OriginalTitle", "PosterUrl", "ReleaseDate", "Status", "Subtitle", "Title", "TrailerUrl" },
                values: new object[,]
                {
                    { 1, 5, 4.8m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Câu chuyện về J. Robert Oppenheimer, nhà vật lý lý thuyết người Mỹ lãnh đạo Dự án Manhattan phát triển vũ khí nguyên tử đầu tiên trên thế giới trong Thế chiến thứ hai.", 1, 180, 2, "Oppenheimer", "https://upload.wikimedia.org/wikipedia/en/4/4a/Oppenheimer_%28film%29.jpg", new DateOnly(2023, 8, 11), "NowShowing", "Tiếng Việt", "Oppenheimer", "https://www.youtube.com/watch?v=keydELfWuMQ" },
                    { 2, 4, 4.7m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Paul Atreides hợp lực cùng Chani và người Fremen để trả thù những kẻ đã hủy diệt gia đình anh, đồng thời đối mặt với lựa chọn nghiệt ngã giữa tình yêu và vận mệnh của vũ trụ.", 2, 166, 2, "Dune: Part Two", "https://upload.wikimedia.org/wikipedia/en/5/52/Dune_Part_Two_poster.jpeg", new DateOnly(2024, 3, 1), "NowShowing", "Tiếng Việt", "Dune: Hành Tinh Cát - Phần Hai", "https://www.youtube.com/watch?v=Way9Dexny3w" },
                    { 3, 5, 4.5m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mai - người phụ nữ gần 40 tuổi làm nghề mát-xa trị liệu mang nhiều vết thương tâm hồn, gặp Dương - chàng trai đào hoa trẻ tuổi quyết tâm theo đuổi cô. Một câu chuyện tình yêu đầy thử thách và định kiến xã hội.", 3, 131, 1, "Mai", "https://upload.wikimedia.org/wikipedia/vi/3/36/Mai_2024_poster.jpg", new DateOnly(2024, 2, 10), "NowShowing", "Tiếng Anh", "Mai", "https://www.youtube.com/watch?v=EX6clvId19s" },
                    { 4, 5, 4.9m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gia đình ông Kim nghèo khó tìm cách từng người một thâm nhập vào làm việc cho gia đình ngài Park giàu sang phú quý, mở ra chuỗi bi kịch tầng lớp châm biếm sâu cay.", 7, 132, 3, "Parasite", "https://upload.wikimedia.org/wikipedia/en/5/53/Parasite_%282019_film%29.png", new DateOnly(2019, 6, 21), "Ended", "Tiếng Việt", "Ký Sinh Trùng", "https://www.youtube.com/watch?v=isOGD_7hNIY" },
                    { 5, 3, 4.6m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jake Sully cùng Neytiri và các con phải rời bỏ quê hương, tìm đến nương náu ở vùng biển của tộc Metkayina khi loài người trở lại đe dọa sự sống của Pandora.", 6, 192, 2, "Avatar: The Way of Water", "https://upload.wikimedia.org/wikipedia/en/5/54/Avatar_The_Way_of_Water_poster.jpg", new DateOnly(2022, 12, 16), "Ended", "Tiếng Việt", "Avatar: Dòng Chảy Của Nước", "https://www.youtube.com/watch?v=6AvFHlKS6OE" },
                    { 6, 2, 4.4m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bộ phim gia đình xúc động về bà Hai 73 tuổi neo đơn ở vùng cao Lâm Đồng và những câu chuyện trăn trở của 5 người con đã trưởng thành khi mẹ gặp nạn.", 5, 138, 1, "Face Off 7: One Wish", "https://upload.wikimedia.org/wikipedia/vi/d/d4/%C3%81p_ph%C3%ADch_ch%C3%ADnh_th%E1%BB%A9c_L%E1%BA%ADt_m%E1%BA%B7t_7.jpg", new DateOnly(2024, 4, 26), "NowShowing", "Tiếng Anh", "Lật Mặt 7: Một Điều Ước", "https://www.youtube.com/watch?v=d1ZHdosjNX8" },
                    { 7, 5, 4.7m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cơ quan Phương sai Thời gian (TVA) lôi kéo Deadpool vào một nhiệm vụ bảo vệ đa vũ trụ, buộc anh phải hợp tác cùng một biến thể bất đắc dĩ của Wolverine.", 9, 128, 2, "Deadpool & Wolverine", "https://upload.wikimedia.org/wikipedia/vi/4/4c/Deadpool_%26_Wolverine_poster.jpg", new DateOnly(2024, 7, 26), "NowShowing", "Tiếng Việt", "Deadpool & Wolverine", "https://www.youtube.com/watch?v=73_1biulkYk" },
                    { 8, 1, 4.8m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cô bé Riley bước vào tuổi dậy thì, kéo theo sự xuất hiện của những cảm xúc mới tinh vi như Lo Âu, Ganh Tị, Xấu Hổ và Chán Nản bên cạnh nhóm Vui Vẻ quen thuộc.", 10, 96, 2, "Inside Out 2", "https://upload.wikimedia.org/wikipedia/en/f/f7/Inside_Out_2_poster.jpg", new DateOnly(2024, 6, 14), "NowShowing", "Lồng tiếng & Phụ đề", "Những Mảnh Ghép Cảm Xúc 2", "https://www.youtube.com/watch?v=8rjvKzYeSp0" },
                    { 9, 3, 4.2m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tác phẩm tái hiện cuộc đời tài hoa, lãng tử của cố nhạc sĩ Trịnh Công Sơn qua những giai đoạn gắn liền với các 'nàng thơ' Dao Ánh, Bích Diễm, Khánh Ly, Michiko.", 8, 136, 1, "Em và Trịnh", "https://upload.wikimedia.org/wikipedia/vi/b/b3/Poster_gioi_thieu_phim_Em_va_Trinh.jpg", new DateOnly(2022, 6, 17), "Ended", "Tiếng Anh", "Em Và Trịnh", "https://www.youtube.com/watch?v=zzik4JB9D1Q" },
                    { 10, 5, 4.1m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bộ phim kỳ ảo tâm linh về Hùng - người đàn ông nắm giữ bí thuật ngải bất tử vượt qua 3 thế kỷ đầy tham vọng, thù hận và sự trừng phạt nghiệt ngã của số phận.", 4, 130, 1, "The Immortal", "https://upload.wikimedia.org/wikipedia/en/b/b9/The_Immortal_%282018_film%29_poster.jpg", new DateOnly(2018, 10, 26), "Ended", "Tiếng Anh", "Người Bất Tử", "https://www.youtube.com/watch?v=6VEVPtFDN_U" },
                    { 11, 3, 4.9m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khi Trái Đất bên bờ diệt vong, một nhóm nhà thám hiểm vũ trụ dấn thân vào hố sâu không gian qua một lỗ sâu mới xuất hiện để tìm kiếm hành tinh mới cho sự sống còn của nhân loại.", 1, 169, 2, "Interstellar", "https://upload.wikimedia.org/wikipedia/en/b/bc/Interstellar_film_poster.jpg", new DateOnly(2014, 11, 7), "Ended", "Tiếng Việt", "Hố Đen Tử Thần", "https://www.youtube.com/watch?v=zSWdZVtXT7E" },
                    { 12, 3, 4.5m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyển thể từ truyện dài cùng tên của nhà văn Nguyễn Nhật Ánh, kể về mối tình đơn phương vừa tha thiết vừa đượm buồn của Ngạn dành cho cô bạn thanh mai trúc mã Hà Lan có đôi mắt biếc.", 4, 117, 1, "Dreamy Eyes", "https://upload.wikimedia.org/wikipedia/vi/4/42/%C3%81p_ph%C3%ADch_phim_M%E1%BA%AFt_bi%E1%BA%BFc.jpg", new DateOnly(2019, 12, 20), "Ended", "Tiếng Anh", "Mắt Biếc", "https://www.youtube.com/watch?v=ITlQ0oU7tDA" },
                    { 13, 2, 4.4m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tác phẩm xoay quanh đại gia đình Giàu - Sang - Phú - Quý tại một con hẻm nghèo ở Sài Gòn, đặc biệt là mối quan hệ mâu thuẫn thế hệ đầy day dứt và tình yêu thương sâu sắc giữa Ba Sang và Quắn.", 3, 128, 1, "Dad, I'm Sorry", "https://upload.wikimedia.org/wikipedia/vi/7/72/Bo_gia_2021_ap_phich.jpg", new DateOnly(2021, 3, 5), "Ended", "Tiếng Anh", "Bố Già", "https://www.youtube.com/watch?v=jluSu8Rw6YE" },
                    { 14, 5, 4.6m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Một nhóm pháp sư, thầy phong thủy và chuyên gia tang lễ hàng đầu Hàn Quốc nhận lời khai quật một ngôi mộ bí ẩn với mức thù lao khổng lồ, vô tình giải thoát một thế lực hắc ám cổ xưa rùng rợn.", 12, 134, 3, "Exhuma", "https://upload.wikimedia.org/wikipedia/en/1/13/Exhuma_film_poster.jpg", new DateOnly(2024, 3, 15), "NowShowing", "Tiếng Việt", "Quật Mộ Trùng Ma", "https://www.youtube.com/watch?v=nO3_h3fJ3-s" },
                    { 15, 2, 4.9m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Miles Morales tái ngộ cùng Gwen Stacy và bị kéo vào đa vũ trụ đầy hiểm họa. Tại đây, Miles phải đối đầu với Liên minh Người Nhện để bảo vệ định mệnh và người thân yêu nhất của mình.", 13, 140, 2, "Spider-Man: Across the Spider-Verse", "https://upload.wikimedia.org/wikipedia/en/b/b4/Spider-Man-_Across_the_Spider-Verse_poster.jpg", new DateOnly(2023, 6, 2), "Ended", "Tiếng Việt", "Người Nhện: Du Hành Vũ Trụ Nhện", "https://www.youtube.com/watch?v=cqGjhVJWtEg" },
                    { 16, 3, 4.9m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sau cú búng tay hủy diệt của Thanos, các Avengers còn sống sót tập hợp lực lượng một lần cuối cùng, thực hiện kế hoạch du hành thời gian đầy táo bạo nhằm khôi phục vũ trụ.", 11, 181, 2, "Avengers: Endgame", "https://upload.wikimedia.org/wikipedia/en/0/0d/Avengers_Endgame_poster.jpg", new DateOnly(2019, 4, 26), "Ended", "Tiếng Việt", "Avengers: Hồi Kết", "https://www.youtube.com/watch?v=TcMBFSGVi1c" },
                    { 17, 1, 4.3m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Po chuẩn bị trở thành Thủ lĩnh Tinh thần của Thung lũng Bình Yên và phải huấn luyện một chiến binh rồng mới, đồng thời đối đầu với mụ phù thủy Tắc Kè Hoa có khả năng biến hình nguy hiểm.", 14, 94, 2, "Kung Fu Panda 4", "https://upload.wikimedia.org/wikipedia/en/7/7f/Kung_Fu_Panda_4_poster.jpg", new DateOnly(2024, 3, 8), "NowShowing", "Lồng tiếng & Phụ đề", "Kung Fu Panda 4", "https://www.youtube.com/watch?v=_inKs4eeHiI" },
                    { 18, 4, 4.3m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Câu chuyện xoay quanh tiệm bánh canh cua của bà Ngọc Nữ và những rắc rối, mâu thuẫn tình cảm trong một gia đình nhiều thế hệ toàn phụ nữ nơi sự kiểm soát thái quá dẫn đến đổ vỡ.", 3, 102, 1, "The House of No Man", "https://upload.wikimedia.org/wikipedia/vi/6/6f/%C3%81p_ph%C3%ADch_phim_Nh%C3%A0_b%C3%A0_N%E1%BB%AF.jpg", new DateOnly(2023, 1, 22), "Ended", "Tiếng Anh", "Nhà Bà Nữ", "https://www.youtube.com/watch?v=684y48YgqjA" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "AgeRatingId", "AverageRating", "CreatedAt", "Description", "DirectorId", "DurationMinutes", "LanguageId", "OriginalTitle", "PosterUrl", "ReleaseDate", "Subtitle", "Title", "TrailerUrl" },
                values: new object[] { 19, 2, 4.7m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cô nữ sinh 17 tuổi Suzume gặp gỡ chàng trai kỳ lạ Souta và bắt đầu hành trình băng qua khắp nước Nhật để đóng lại những cánh cửa giải phóng tai ương thảm họa bảo vệ sự bình yên nhân loại.", 15, 122, 4, "Suzume", "https://upload.wikimedia.org/wikipedia/en/7/7f/Suzume_no_Tojimari_poster.jpg", new DateOnly(2023, 3, 10), "Tiếng Việt", "Khóa Chặt Cửa Nào Suzume", "https://www.youtube.com/watch?v=5pY4qVz9rEQ" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Avatar", "CreatedAt", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "Gender", "GoogleId", "IsActive", "PasswordHash", "Phone", "RoleId", "UpdatedAt" },
                values: new object[] { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "admin@cinema.local", true, "System Admin", null, null, true, "AQAAAAEAAYagAAAAENS3DOHbkUObJNrLWeaXshnI43hVryVZB/xVqXl+RRCO+rT1ek+EVigjK5BS5n9leQ==", null, 2, null });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "HireDate", "IsActive", "PositionId", "UserId" },
                values: new object[] { 1, new DateOnly(2026, 1, 1), true, 1, 1 });

            migrationBuilder.InsertData(
                table: "MovieCast",
                columns: new[] { "ActorId", "MovieId", "CharacterName" },
                values: new object[,]
                {
                    { 1, 1, "J. Robert Oppenheimer" },
                    { 2, 1, "Lewis Strauss" },
                    { 3, 2, "Paul Atreides" },
                    { 4, 2, "Chani" },
                    { 5, 3, "Ông Hoàng" },
                    { 6, 3, "Trùng Dương (Sâu)" },
                    { 7, 3, "Bình Minh" },
                    { 8, 3, "Mai" },
                    { 9, 4, "Kim Ki-taek" },
                    { 10, 4, "Kim Ki-woo" },
                    { 11, 5, "Jake Sully" },
                    { 12, 5, "Neytiri" },
                    { 15, 6, "Ba Tràng" },
                    { 18, 6, "Tư Hậu" },
                    { 13, 7, "Wade Wilson / Deadpool" },
                    { 14, 7, "Logan / Wolverine" },
                    { 21, 8, "Joy (Vui Vẻ)" },
                    { 22, 8, "Anxiety (Lo Âu)" },
                    { 19, 9, "Trịnh Công Sơn (thời trẻ)" },
                    { 20, 9, "Dao Ánh" },
                    { 17, 10, "Liên" },
                    { 18, 10, "Hùng" },
                    { 23, 11, "Joseph Cooper" },
                    { 24, 11, "Dr. Amelia Brand" },
                    { 25, 12, "Ngạn" },
                    { 26, 12, "Hà Lan" },
                    { 5, 13, "Ba Sang" },
                    { 6, 13, "Quắn" },
                    { 36, 13, "Cẩm Lệ" },
                    { 27, 14, "Kim Sang-deok" },
                    { 28, 14, "Lee Hwa-rim" },
                    { 29, 14, "Yoon Bong-gil" },
                    { 30, 15, "Miles Morales / Spider-Man" },
                    { 31, 15, "Gwen Stacy / Spider-Woman" },
                    { 2, 16, "Tony Stark / Iron Man" },
                    { 32, 16, "Steve Rogers / Captain America" },
                    { 33, 16, "Natasha Romanoff / Black Widow" },
                    { 34, 17, "Po" },
                    { 5, 18, "Phú Nhuận" },
                    { 7, 18, "Ngọc Nhi" },
                    { 35, 18, "John" },
                    { 36, 18, "Bà Ngọc Nữ" },
                    { 37, 19, "Suzume Iwato" },
                    { 38, 19, "Souta Munakata" }
                });

            migrationBuilder.InsertData(
                table: "MovieGenres",
                columns: new[] { "GenreId", "MovieId" },
                values: new object[,]
                {
                    { 3, 1 },
                    { 8, 1 },
                    { 1, 2 },
                    { 3, 2 },
                    { 5, 2 },
                    { 3, 3 },
                    { 7, 3 },
                    { 8, 3 },
                    { 3, 4 },
                    { 4, 4 },
                    { 8, 4 },
                    { 1, 5 },
                    { 5, 5 },
                    { 2, 6 },
                    { 3, 6 },
                    { 1, 7 },
                    { 2, 7 },
                    { 5, 7 },
                    { 2, 8 },
                    { 6, 8 },
                    { 8, 8 },
                    { 3, 9 },
                    { 7, 9 },
                    { 4, 10 },
                    { 8, 10 },
                    { 3, 11 },
                    { 5, 11 },
                    { 9, 11 },
                    { 3, 12 },
                    { 7, 12 },
                    { 2, 13 },
                    { 3, 13 },
                    { 8, 13 },
                    { 4, 14 },
                    { 8, 14 },
                    { 10, 14 },
                    { 1, 15 },
                    { 5, 15 },
                    { 6, 15 },
                    { 1, 16 },
                    { 5, 16 },
                    { 9, 16 },
                    { 1, 17 },
                    { 2, 17 },
                    { 6, 17 },
                    { 2, 18 },
                    { 3, 18 },
                    { 8, 18 },
                    { 6, 19 },
                    { 9, 19 },
                    { 10, 19 }
                });

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "SeatId", "CoupleId", "IsActive", "IsAisle", "RoomId", "SeatNumber", "SeatRow", "SeatTypeId" },
                values: new object[,]
                {
                    { 1, null, true, false, 1, 1, "A", 1 },
                    { 2, null, true, false, 1, 2, "A", 1 },
                    { 3, null, true, false, 1, 3, "A", 1 },
                    { 4, null, true, false, 1, 4, "A", 1 },
                    { 5, null, true, false, 1, 5, "A", 1 },
                    { 6, null, true, false, 1, 6, "A", 1 },
                    { 7, null, true, false, 1, 7, "A", 1 },
                    { 8, null, true, false, 1, 8, "A", 1 },
                    { 9, null, true, false, 1, 9, "A", 1 },
                    { 10, null, true, false, 1, 10, "A", 1 },
                    { 11, null, true, false, 1, 1, "B", 1 },
                    { 12, null, true, false, 1, 2, "B", 1 },
                    { 13, null, true, false, 1, 3, "B", 1 },
                    { 14, null, true, false, 1, 4, "B", 1 },
                    { 15, null, true, false, 1, 5, "B", 1 },
                    { 16, null, true, false, 1, 6, "B", 1 },
                    { 17, null, true, false, 1, 7, "B", 1 },
                    { 18, null, true, false, 1, 8, "B", 1 },
                    { 19, null, true, false, 1, 9, "B", 1 },
                    { 20, null, true, false, 1, 10, "B", 1 },
                    { 21, null, true, false, 1, 1, "C", 2 },
                    { 22, null, true, false, 1, 2, "C", 2 },
                    { 23, null, true, false, 1, 3, "C", 2 },
                    { 24, null, true, false, 1, 4, "C", 2 },
                    { 25, null, true, false, 1, 5, "C", 2 },
                    { 26, null, true, false, 1, 6, "C", 2 },
                    { 27, null, true, false, 1, 7, "C", 2 },
                    { 28, null, true, false, 1, 8, "C", 2 },
                    { 29, null, true, false, 1, 9, "C", 2 },
                    { 30, null, true, false, 1, 10, "C", 2 },
                    { 31, null, true, false, 1, 1, "D", 2 },
                    { 32, null, true, false, 1, 2, "D", 2 },
                    { 33, null, true, false, 1, 3, "D", 2 },
                    { 34, null, true, false, 1, 4, "D", 2 },
                    { 35, null, true, false, 1, 5, "D", 2 },
                    { 36, null, true, false, 1, 6, "D", 2 },
                    { 37, null, true, false, 1, 7, "D", 2 },
                    { 38, null, true, false, 1, 8, "D", 2 },
                    { 39, null, true, false, 1, 9, "D", 2 },
                    { 40, null, true, false, 1, 10, "D", 2 },
                    { 41, new Guid("c0000000-0000-0000-0001-000000005001"), true, false, 1, 1, "E", 3 },
                    { 42, new Guid("c0000000-0000-0000-0001-000000005001"), true, false, 1, 2, "E", 3 },
                    { 43, new Guid("c0000000-0000-0000-0001-000000005002"), true, false, 1, 3, "E", 3 },
                    { 44, new Guid("c0000000-0000-0000-0001-000000005002"), true, false, 1, 4, "E", 3 },
                    { 45, new Guid("c0000000-0000-0000-0001-000000005003"), true, false, 1, 5, "E", 3 },
                    { 46, new Guid("c0000000-0000-0000-0001-000000005003"), true, false, 1, 6, "E", 3 },
                    { 47, new Guid("c0000000-0000-0000-0001-000000005004"), true, false, 1, 7, "E", 3 },
                    { 48, new Guid("c0000000-0000-0000-0001-000000005004"), true, false, 1, 8, "E", 3 },
                    { 49, new Guid("c0000000-0000-0000-0001-000000005005"), true, false, 1, 9, "E", 3 },
                    { 50, new Guid("c0000000-0000-0000-0001-000000005005"), true, false, 1, 10, "E", 3 },
                    { 51, null, true, false, 2, 1, "A", 1 },
                    { 52, null, true, false, 2, 2, "A", 1 },
                    { 53, null, true, false, 2, 3, "A", 1 },
                    { 54, null, true, false, 2, 4, "A", 1 },
                    { 55, null, true, false, 2, 5, "A", 1 },
                    { 56, null, true, false, 2, 6, "A", 1 },
                    { 57, null, true, false, 2, 7, "A", 1 },
                    { 58, null, true, false, 2, 8, "A", 1 },
                    { 59, null, true, false, 2, 9, "A", 1 },
                    { 60, null, true, false, 2, 10, "A", 1 },
                    { 61, null, true, false, 2, 1, "B", 1 },
                    { 62, null, true, false, 2, 2, "B", 1 },
                    { 63, null, true, false, 2, 3, "B", 1 },
                    { 64, null, true, false, 2, 4, "B", 1 },
                    { 65, null, true, false, 2, 5, "B", 1 },
                    { 66, null, true, false, 2, 6, "B", 1 },
                    { 67, null, true, false, 2, 7, "B", 1 },
                    { 68, null, true, false, 2, 8, "B", 1 },
                    { 69, null, true, false, 2, 9, "B", 1 },
                    { 70, null, true, false, 2, 10, "B", 1 },
                    { 71, null, true, false, 2, 1, "C", 2 },
                    { 72, null, true, false, 2, 2, "C", 2 },
                    { 73, null, true, false, 2, 3, "C", 2 },
                    { 74, null, true, false, 2, 4, "C", 2 },
                    { 75, null, true, false, 2, 5, "C", 2 },
                    { 76, null, true, false, 2, 6, "C", 2 },
                    { 77, null, true, false, 2, 7, "C", 2 },
                    { 78, null, true, false, 2, 8, "C", 2 },
                    { 79, null, true, false, 2, 9, "C", 2 },
                    { 80, null, true, false, 2, 10, "C", 2 },
                    { 81, null, true, false, 2, 1, "D", 2 },
                    { 82, null, true, false, 2, 2, "D", 2 },
                    { 83, null, true, false, 2, 3, "D", 2 },
                    { 84, null, true, false, 2, 4, "D", 2 },
                    { 85, null, true, false, 2, 5, "D", 2 },
                    { 86, null, true, false, 2, 6, "D", 2 },
                    { 87, null, true, false, 2, 7, "D", 2 },
                    { 88, null, true, false, 2, 8, "D", 2 },
                    { 89, null, true, false, 2, 9, "D", 2 },
                    { 90, null, true, false, 2, 10, "D", 2 },
                    { 91, new Guid("c0000000-0000-0000-0002-000000005001"), true, false, 2, 1, "E", 3 },
                    { 92, new Guid("c0000000-0000-0000-0002-000000005001"), true, false, 2, 2, "E", 3 },
                    { 93, new Guid("c0000000-0000-0000-0002-000000005002"), true, false, 2, 3, "E", 3 },
                    { 94, new Guid("c0000000-0000-0000-0002-000000005002"), true, false, 2, 4, "E", 3 },
                    { 95, new Guid("c0000000-0000-0000-0002-000000005003"), true, false, 2, 5, "E", 3 },
                    { 96, new Guid("c0000000-0000-0000-0002-000000005003"), true, false, 2, 6, "E", 3 },
                    { 97, new Guid("c0000000-0000-0000-0002-000000005004"), true, false, 2, 7, "E", 3 },
                    { 98, new Guid("c0000000-0000-0000-0002-000000005004"), true, false, 2, 8, "E", 3 },
                    { 99, new Guid("c0000000-0000-0000-0002-000000005005"), true, false, 2, 9, "E", 3 },
                    { 100, new Guid("c0000000-0000-0000-0002-000000005005"), true, false, 2, 10, "E", 3 },
                    { 101, null, true, false, 3, 1, "A", 1 },
                    { 102, null, true, false, 3, 2, "A", 1 },
                    { 103, null, true, false, 3, 3, "A", 1 },
                    { 104, null, true, false, 3, 4, "A", 1 },
                    { 105, null, true, false, 3, 5, "A", 1 },
                    { 106, null, true, false, 3, 6, "A", 1 },
                    { 107, null, true, false, 3, 7, "A", 1 },
                    { 108, null, true, false, 3, 8, "A", 1 },
                    { 109, null, true, false, 3, 9, "A", 1 },
                    { 110, null, true, false, 3, 10, "A", 1 },
                    { 111, null, true, false, 3, 1, "B", 2 },
                    { 112, null, true, false, 3, 2, "B", 2 },
                    { 113, null, true, false, 3, 3, "B", 2 },
                    { 114, null, true, false, 3, 4, "B", 2 },
                    { 115, null, true, false, 3, 5, "B", 2 },
                    { 116, null, true, false, 3, 6, "B", 2 },
                    { 117, null, true, false, 3, 7, "B", 2 },
                    { 118, null, true, false, 3, 8, "B", 2 },
                    { 119, null, true, false, 3, 9, "B", 2 },
                    { 120, null, true, false, 3, 10, "B", 2 },
                    { 121, null, true, false, 3, 1, "C", 2 },
                    { 122, null, true, false, 3, 2, "C", 2 },
                    { 123, null, true, false, 3, 3, "C", 2 },
                    { 124, null, true, false, 3, 4, "C", 2 },
                    { 125, null, true, false, 3, 5, "C", 2 },
                    { 126, null, true, false, 3, 6, "C", 2 },
                    { 127, null, true, false, 3, 7, "C", 2 },
                    { 128, null, true, false, 3, 8, "C", 2 },
                    { 129, null, true, false, 3, 9, "C", 2 },
                    { 130, null, true, false, 3, 10, "C", 2 },
                    { 131, new Guid("c0000000-0000-0000-0003-000000004001"), true, false, 3, 1, "D", 3 },
                    { 132, new Guid("c0000000-0000-0000-0003-000000004001"), true, false, 3, 2, "D", 3 },
                    { 133, new Guid("c0000000-0000-0000-0003-000000004002"), true, false, 3, 3, "D", 3 },
                    { 134, new Guid("c0000000-0000-0000-0003-000000004002"), true, false, 3, 4, "D", 3 },
                    { 135, new Guid("c0000000-0000-0000-0003-000000004003"), true, false, 3, 5, "D", 3 },
                    { 136, new Guid("c0000000-0000-0000-0003-000000004003"), true, false, 3, 6, "D", 3 },
                    { 137, new Guid("c0000000-0000-0000-0003-000000004004"), true, false, 3, 7, "D", 3 },
                    { 138, new Guid("c0000000-0000-0000-0003-000000004004"), true, false, 3, 8, "D", 3 },
                    { 139, new Guid("c0000000-0000-0000-0003-000000004005"), true, false, 3, 9, "D", 3 },
                    { 140, new Guid("c0000000-0000-0000-0003-000000004005"), true, false, 3, 10, "D", 3 },
                    { 141, null, true, false, 4, 1, "A", 1 },
                    { 142, null, true, false, 4, 2, "A", 1 },
                    { 143, null, true, false, 4, 3, "A", 1 },
                    { 144, null, true, false, 4, 4, "A", 1 },
                    { 145, null, true, false, 4, 5, "A", 1 },
                    { 146, null, true, false, 4, 6, "A", 1 },
                    { 147, null, true, false, 4, 7, "A", 1 },
                    { 148, null, true, false, 4, 8, "A", 1 },
                    { 149, null, true, false, 4, 9, "A", 1 },
                    { 150, null, true, false, 4, 10, "A", 1 },
                    { 151, null, true, false, 4, 1, "B", 1 },
                    { 152, null, true, false, 4, 2, "B", 1 },
                    { 153, null, true, false, 4, 3, "B", 1 },
                    { 154, null, true, false, 4, 4, "B", 1 },
                    { 155, null, true, false, 4, 5, "B", 1 },
                    { 156, null, true, false, 4, 6, "B", 1 },
                    { 157, null, true, false, 4, 7, "B", 1 },
                    { 158, null, true, false, 4, 8, "B", 1 },
                    { 159, null, true, false, 4, 9, "B", 1 },
                    { 160, null, true, false, 4, 10, "B", 1 },
                    { 161, null, true, false, 4, 1, "C", 2 },
                    { 162, null, true, false, 4, 2, "C", 2 },
                    { 163, null, true, false, 4, 3, "C", 2 },
                    { 164, null, true, false, 4, 4, "C", 2 },
                    { 165, null, true, false, 4, 5, "C", 2 },
                    { 166, null, true, false, 4, 6, "C", 2 },
                    { 167, null, true, false, 4, 7, "C", 2 },
                    { 168, null, true, false, 4, 8, "C", 2 },
                    { 169, null, true, false, 4, 9, "C", 2 },
                    { 170, null, true, false, 4, 10, "C", 2 },
                    { 171, null, true, false, 4, 1, "D", 2 },
                    { 172, null, true, false, 4, 2, "D", 2 },
                    { 173, null, true, false, 4, 3, "D", 2 },
                    { 174, null, true, false, 4, 4, "D", 2 },
                    { 175, null, true, false, 4, 5, "D", 2 },
                    { 176, null, true, false, 4, 6, "D", 2 },
                    { 177, null, true, false, 4, 7, "D", 2 },
                    { 178, null, true, false, 4, 8, "D", 2 },
                    { 179, null, true, false, 4, 9, "D", 2 },
                    { 180, null, true, false, 4, 10, "D", 2 },
                    { 181, null, true, false, 4, 1, "E", 2 },
                    { 182, null, true, false, 4, 2, "E", 2 },
                    { 183, null, true, false, 4, 3, "E", 2 },
                    { 184, null, true, false, 4, 4, "E", 2 },
                    { 185, null, true, false, 4, 5, "E", 2 },
                    { 186, null, true, false, 4, 6, "E", 2 },
                    { 187, null, true, false, 4, 7, "E", 2 },
                    { 188, null, true, false, 4, 8, "E", 2 },
                    { 189, null, true, false, 4, 9, "E", 2 },
                    { 190, null, true, false, 4, 10, "E", 2 },
                    { 191, new Guid("c0000000-0000-0000-0004-000000006001"), true, false, 4, 1, "F", 3 },
                    { 192, new Guid("c0000000-0000-0000-0004-000000006001"), true, false, 4, 2, "F", 3 },
                    { 193, new Guid("c0000000-0000-0000-0004-000000006002"), true, false, 4, 3, "F", 3 },
                    { 194, new Guid("c0000000-0000-0000-0004-000000006002"), true, false, 4, 4, "F", 3 },
                    { 195, new Guid("c0000000-0000-0000-0004-000000006003"), true, false, 4, 5, "F", 3 },
                    { 196, new Guid("c0000000-0000-0000-0004-000000006003"), true, false, 4, 6, "F", 3 },
                    { 197, new Guid("c0000000-0000-0000-0004-000000006004"), true, false, 4, 7, "F", 3 },
                    { 198, new Guid("c0000000-0000-0000-0004-000000006004"), true, false, 4, 8, "F", 3 },
                    { 199, new Guid("c0000000-0000-0000-0004-000000006005"), true, false, 4, 9, "F", 3 },
                    { 200, new Guid("c0000000-0000-0000-0004-000000006005"), true, false, 4, 10, "F", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgeRatings_Code",
                table: "AgeRatings",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingCombos_Booking",
                table: "BookingCombos",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCombos_ComboId",
                table: "BookingCombos",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingCode",
                table: "Bookings",
                column: "BookingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Channel",
                table: "Bookings",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Employee",
                table: "Bookings",
                column: "CreatedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Showtime",
                table: "Bookings",
                column: "ShowtimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Status",
                table: "Bookings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_User",
                table: "Bookings",
                columns: new[] { "UserId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_BookingTickets_BookingId",
                table: "BookingTickets",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTickets_ShowtimeSeatId",
                table: "BookingTickets",
                column: "ShowtimeSeatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CinemaRooms_RoomName",
                table: "CinemaRooms",
                column: "RoomName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CinemaRooms_ScreenTypeId",
                table: "CinemaRooms",
                column: "ScreenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProfiles_Tier",
                table: "CustomerProfiles",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Position",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Name",
                table: "Languages",
                column: "Name",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_MembershipTiers_TierName",
                table: "MembershipTiers",
                column: "TierName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieCast_Actor",
                table: "MovieCast",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_GenreId",
                table: "MovieGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_AgeRating",
                table: "Movies",
                column: "AgeRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Director",
                table: "Movies",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Language",
                table: "Movies",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ReleaseDate",
                table: "Movies",
                column: "ReleaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Status",
                table: "Movies",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Booking",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_Name",
                table: "Positions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ReplacedByTokenId",
                table: "RefreshTokens",
                column: "ReplacedByTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_User",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Movie",
                table: "Reviews",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Movie_Review",
                table: "Reviews",
                columns: new[] { "UserId", "MovieId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScreenTypes_Name",
                table: "ScreenTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_SeatType",
                table: "Seats",
                column: "SeatTypeId");

            migrationBuilder.CreateIndex(
                name: "UQ_Room_Seat",
                table: "Seats",
                columns: new[] { "RoomId", "SeatRow", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeatTypes_Name",
                table: "SeatTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Showtime_Movie",
                table: "Showtimes",
                columns: new[] { "MovieId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Showtime_Room_Time",
                table: "Showtimes",
                columns: new[] { "RoomId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ShowtimeSeats_HeldByBookingId",
                table: "ShowtimeSeats",
                column: "HeldByBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowtimeSeats_HeldByUserId",
                table: "ShowtimeSeats",
                column: "HeldByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowtimeSeats_SeatId",
                table: "ShowtimeSeats",
                column: "SeatId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowtimeSeats_Status",
                table: "ShowtimeSeats",
                columns: new[] { "ShowtimeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "UQ_Showtime_Seat",
                table: "ShowtimeSeats",
                columns: new[] { "ShowtimeId", "SeatId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleId",
                table: "Users",
                column: "GoogleId",
                unique: true,
                filter: "\"GoogleId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationTokens_User_Purpose",
                table: "UserVerificationTokens",
                columns: new[] { "UserId", "Purpose" });

            migrationBuilder.CreateIndex(
                name: "UQ_UserVerificationTokens_TokenHash",
                table: "UserVerificationTokens",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingCombos");

            migrationBuilder.DropTable(
                name: "BookingTickets");

            migrationBuilder.DropTable(
                name: "CinemaSettings");

            migrationBuilder.DropTable(
                name: "CustomerProfiles");

            migrationBuilder.DropTable(
                name: "LoyaltyPointEntries");

            migrationBuilder.DropTable(
                name: "MovieCast");

            migrationBuilder.DropTable(
                name: "MovieGenres");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "UserVerificationTokens");

            migrationBuilder.DropTable(
                name: "Combos");

            migrationBuilder.DropTable(
                name: "ShowtimeSeats");

            migrationBuilder.DropTable(
                name: "MembershipTiers");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Showtimes");

            migrationBuilder.DropTable(
                name: "SeatTypes");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CinemaRooms");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ScreenTypes");

            migrationBuilder.DropTable(
                name: "AgeRatings");

            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
