using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegionScope.Migrations
{
    /// <inheritdoc />
    public partial class AddIndicatorsAndObservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Indicators",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indicators", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    IndicatorCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RetrievedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => new { x.CountryCode, x.IndicatorCode, x.Year });
                    table.ForeignKey(
                        name: "FK_Observations_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Observations_Indicators_IndicatorCode",
                        column: x => x.IndicatorCode,
                        principalTable: "Indicators",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Indicators",
                columns: new[] { "Code", "Name", "Unit" },
                values: new object[,]
                {
                    { "gdp_per_capita", "GDP per capita", "EUR per person" },
                    { "population", "Population", "persons" },
                    { "unemployment_rate", "Unemployment rate", "percentage" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Observations_IndicatorCode_Year",
                table: "Observations",
                columns: new[] { "IndicatorCode", "Year" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropTable(
                name: "Indicators");
        }
    }
}
