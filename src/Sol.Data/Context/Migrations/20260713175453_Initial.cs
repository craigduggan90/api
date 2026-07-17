using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sol.Data.Context.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    idempotency_key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    type = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<int>(type: "INTEGER", nullable: false),
                    last_event_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    parameters = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    error_code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    error_message = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    cursor = table.Column<long>(type: "INTEGER", nullable: false),
                    date_created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_modified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    date_deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_jobs_cursor",
                table: "jobs",
                column: "cursor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_jobs_date_created",
                table: "jobs",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_date_deleted",
                table: "jobs",
                column: "date_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_date_modified",
                table: "jobs",
                column: "date_modified");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_idempotency_key",
                table: "jobs",
                column: "idempotency_key",
                unique: true,
                filter: "date_deleted IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobs");
        }
    }
}
