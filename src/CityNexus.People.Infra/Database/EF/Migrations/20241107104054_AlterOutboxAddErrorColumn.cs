using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityNexus.People.Infra.Database.EF.Migrations
{
    /// <inheritdoc />
    public partial class AlterOutboxAddErrorColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error",
                table: "outbox",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "error", table: "outbox");
        }
    }
}
