using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "producto_pedido",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo_barra = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    referencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    marca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    talla = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    color = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fabricante = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    precio_detal = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    costo = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    creado_por_codigo_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fecha_creacion_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_pedido", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "registro_precio_pedido",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo_barra = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    producto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    sucursal = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    precio_sistema = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    precio_verificado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_precio_pedido", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_producto_pedido_codigo_barra",
                table: "producto_pedido",
                column: "codigo_barra",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "producto_pedido");

            migrationBuilder.DropTable(
                name: "registro_precio_pedido");
        }
    }
}
