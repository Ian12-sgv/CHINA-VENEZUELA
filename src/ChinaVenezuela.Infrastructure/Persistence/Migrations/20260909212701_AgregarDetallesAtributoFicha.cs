using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDetallesAtributoFicha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "color_para_fabricar",
                table: "ficha_tecnica_atributo",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "composicion_tela",
                table: "ficha_tecnica_atributo",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "curva_talla",
                table: "ficha_tecnica_atributo",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "marca_producto",
                table: "ficha_tecnica_atributo",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color_para_fabricar",
                table: "ficha_tecnica_atributo");

            migrationBuilder.DropColumn(
                name: "composicion_tela",
                table: "ficha_tecnica_atributo");

            migrationBuilder.DropColumn(
                name: "curva_talla",
                table: "ficha_tecnica_atributo");

            migrationBuilder.DropColumn(
                name: "marca_producto",
                table: "ficha_tecnica_atributo");
        }
    }
}
