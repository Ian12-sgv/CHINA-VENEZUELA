using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCompraRecibida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "compra_recibida",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contenedor_compartido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    nombre_contenedor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero_contenedor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    empresa = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    fecha_salida = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_llegada = table.Column<DateOnly>(type: "date", nullable: true),
                    aduana = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    puerto_llegada = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    marca_bultos = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    fecha_creacion_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compra_recibida", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "registro_auditoria",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_entidad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entidad_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    valores_antes_json = table.Column<string>(type: "jsonb", nullable: true),
                    valores_despues_json = table.Column<string>(type: "jsonb", nullable: true),
                    fecha_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_auditoria", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_compra_recibida_numero_contenedor",
                table: "compra_recibida",
                column: "numero_contenedor");

            migrationBuilder.CreateIndex(
                name: "ix_registro_auditoria_tipo_entidad_entidad_id",
                table: "registro_auditoria",
                columns: new[] { "tipo_entidad", "entidad_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "compra_recibida");

            migrationBuilder.DropTable(
                name: "registro_auditoria");
        }
    }
}
