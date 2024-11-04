using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityNexus.People.Infra.Database.EF.Migrations
{
    /// <inheritdoc />
    public partial class OutboxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_name = table.Column<string>(type: "text", nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    processed_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_outbox_event_name",
                table: "outbox",
                column: "event_name"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "outbox");
        }
    }
}
