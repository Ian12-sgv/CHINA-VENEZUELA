using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDosTiposImagenProductoPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_producto_pedido_imagen_producto_pedido_id",
                table: "producto_pedido_imagen");

            migrationBuilder.AddColumn<string>(
                name: "tipo",
                table: "producto_pedido_imagen",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "ProductoTerminado");

            migrationBuilder.CreateIndex(
                name: "IX_producto_pedido_imagen_producto_pedido_id_tipo",
                table: "producto_pedido_imagen",
                columns: new[] { "producto_pedido_id", "tipo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_producto_pedido_imagen_producto_pedido_id_tipo",
                table: "producto_pedido_imagen");

            migrationBuilder.DropColumn(
                name: "tipo",
                table: "producto_pedido_imagen");

            migrationBuilder.CreateIndex(
                name: "IX_producto_pedido_imagen_producto_pedido_id",
                table: "producto_pedido_imagen",
                column: "producto_pedido_id",
                unique: true);
        }
    }
}
