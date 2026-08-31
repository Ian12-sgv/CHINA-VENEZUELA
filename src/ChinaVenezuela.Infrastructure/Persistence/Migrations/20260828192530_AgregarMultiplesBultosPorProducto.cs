using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMultiplesBultosPorProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "producto_pedido_bulto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_pedido_id = table.Column<Guid>(type: "uuid", nullable: false),
                    marca_bulto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_pedido_bulto", x => x.id);
                    table.ForeignKey(
                        name: "FK_producto_pedido_bulto_marca_bulto_marca_bulto_id",
                        column: x => x.marca_bulto_id,
                        principalTable: "marca_bulto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_producto_pedido_bulto_producto_pedido_producto_pedido_id",
                        column: x => x.producto_pedido_id,
                        principalTable: "producto_pedido",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_producto_pedido_bulto_marca_bulto_id",
                table: "producto_pedido_bulto",
                column: "marca_bulto_id");

            migrationBuilder.CreateIndex(
                name: "IX_producto_pedido_bulto_producto_pedido_id_marca_bulto_id",
                table: "producto_pedido_bulto",
                columns: new[] { "producto_pedido_id", "marca_bulto_id" },
                unique: true);
            migrationBuilder.Sql("""
                INSERT INTO marca_bulto (id, nombre)
                SELECT md5('legacy-marca-bulto:' || lower(trim(p.marca_bulto)))::uuid, trim(p.marca_bulto)
                FROM producto_pedido AS p
                WHERE p.marca_bulto IS NOT NULL
                  AND btrim(p.marca_bulto) <> ''
                  AND p.cantidad_bulto IS NOT NULL
                  AND p.cantidad_bulto > 0
                  AND NOT EXISTS (
                      SELECT 1
                      FROM marca_bulto AS m
                      WHERE lower(m.nombre) = lower(trim(p.marca_bulto))
                  );

                INSERT INTO producto_pedido_bulto (id, producto_pedido_id, marca_bulto_id, cantidad)
                SELECT md5('legacy-producto-pedido-bulto:' || p.id::text || ':' || m.id::text)::uuid,
                       p.id,
                       m.id,
                       p.cantidad_bulto
                FROM producto_pedido AS p
                INNER JOIN marca_bulto AS m
                    ON lower(m.nombre) = lower(trim(p.marca_bulto))
                WHERE p.marca_bulto IS NOT NULL
                  AND btrim(p.marca_bulto) <> ''
                  AND p.cantidad_bulto IS NOT NULL
                  AND p.cantidad_bulto > 0
                  AND NOT EXISTS (
                      SELECT 1
                      FROM producto_pedido_bulto AS pb
                      WHERE pb.producto_pedido_id = p.id
                        AND pb.marca_bulto_id = m.id
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "producto_pedido_bulto");
        }
    }
}
