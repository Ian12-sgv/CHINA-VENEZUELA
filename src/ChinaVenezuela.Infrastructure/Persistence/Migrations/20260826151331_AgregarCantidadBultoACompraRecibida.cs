using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCantidadBultoACompraRecibida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cantidad_bulto",
                table: "compra_recibida",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cantidad_bulto",
                table: "compra_recibida");
        }
    }
}
