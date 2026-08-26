using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPrecioRmbTotalYCantidadDoz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cantidad_doz",
                table: "producto_pedido",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_rmb",
                table: "producto_pedido",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_rmb",
                table: "producto_pedido",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cantidad_doz",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "precio_rmb",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "total_rmb",
                table: "producto_pedido");
        }
    }
}
