using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCorreoAUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "correo",
                table: "usuario",
                type: "character varying(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "usuario",
                keyColumn: "codigo_usuario",
                keyValue: "MARTHA",
                column: "correo",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ux_usuario_correo",
                table: "usuario",
                column: "correo",
                unique: true,
                filter: "correo IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_usuario_correo",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "correo",
                table: "usuario");
        }
    }
}
