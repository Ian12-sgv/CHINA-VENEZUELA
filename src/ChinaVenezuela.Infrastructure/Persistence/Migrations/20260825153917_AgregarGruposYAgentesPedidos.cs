using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarGruposYAgentesPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tipo_pedido",
                table: "producto_pedido",
                newName: "tipo_producto");

            migrationBuilder.CreateTable(
                name: "agente_pedido",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agente_pedido", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    creado_por_codigo_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fecha_creacion_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pedidos_grupos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pedido_id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_pedido_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha_creacion_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos_grupos", x => x.id);
                    table.ForeignKey(
                        name: "FK_pedidos_grupos_pedidos_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pedidos_grupos_producto_pedido_producto_pedido_id",
                        column: x => x.producto_pedido_id,
                        principalTable: "producto_pedido",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                INSERT INTO agente_pedido (id, nombre)
                SELECT gen_random_uuid(), TRIM(p.agente)
                FROM producto_pedido p
                WHERE p.agente IS NOT NULL
                  AND BTRIM(p.agente) <> ''
                  AND NOT EXISTS (
                      SELECT 1 FROM agente_pedido a WHERE a.nombre = TRIM(p.agente)
                  );");
            migrationBuilder.CreateIndex(
                name: "IX_agente_pedido_nombre",
                table: "agente_pedido",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_nombre",
                table: "pedidos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_grupos_pedido_id",
                table: "pedidos_grupos",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_grupos_producto_pedido_id",
                table: "pedidos_grupos",
                column: "producto_pedido_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agente_pedido");

            migrationBuilder.DropTable(
                name: "pedidos_grupos");

            migrationBuilder.DropTable(
                name: "pedidos");

            migrationBuilder.RenameColumn(
                name: "tipo_producto",
                table: "producto_pedido",
                newName: "tipo_pedido");
        }
    }
}
