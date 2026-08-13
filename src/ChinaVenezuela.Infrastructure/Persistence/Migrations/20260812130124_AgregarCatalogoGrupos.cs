using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCatalogoGrupos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "grupo",
                columns: table => new
                {
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupo", x => x.nombre);
                });

            migrationBuilder.InsertData(
                table: "grupo",
                column: "nombre",
                value: "Prueba");

            migrationBuilder.CreateIndex(
                name: "ux_usuario_nombre",
                table: "usuario",
                column: "nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grupo");

            migrationBuilder.DropIndex(
                name: "ux_usuario_nombre",
                table: "usuario");
        }
    }
}
