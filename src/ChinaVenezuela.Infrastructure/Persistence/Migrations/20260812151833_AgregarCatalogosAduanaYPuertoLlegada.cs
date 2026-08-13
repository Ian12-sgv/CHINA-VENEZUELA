using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCatalogosAduanaYPuertoLlegada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aduana",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aduana", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "puerto_llegada",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_puerto_llegada", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aduana_nombre",
                table: "aduana",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_puerto_llegada_nombre",
                table: "puerto_llegada",
                column: "nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aduana");

            migrationBuilder.DropTable(
                name: "puerto_llegada");
        }
    }
}
