using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Software_Engineering.Migrations
{
    /// <inheritdoc />
    public partial class AddCsvUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CsvUserId",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PressureSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientUserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecordedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MatrixJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PeakPressure = table.Column<int>(type: "int", nullable: false),
                    ContactAreaPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CoefficientOfVariation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RiskScore = table.Column<int>(type: "int", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PressureSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PressureSessions_PatientUserId",
                table: "PressureSessions",
                column: "PatientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PressureSessions_RecordedDate",
                table: "PressureSessions",
                column: "RecordedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PressureSessions");

            migrationBuilder.DropColumn(
                name: "CsvUserId",
                table: "AppUsers");
        }
    }
}
