using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDatosLogisticosProductoPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "agente",
                table: "producto_pedido",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "cantidad_unidades",
                table: "producto_pedido",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "composicion_tela",
                table: "producto_pedido",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "marca_bulto",
                table: "producto_pedido",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "pack_por_caja",
                table: "producto_pedido",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_pedido",
                table: "producto_pedido",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "agente",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "cantidad_unidades",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "composicion_tela",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "marca_bulto",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "pack_por_caja",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "tipo_pedido",
                table: "producto_pedido");
        }
    }
}
