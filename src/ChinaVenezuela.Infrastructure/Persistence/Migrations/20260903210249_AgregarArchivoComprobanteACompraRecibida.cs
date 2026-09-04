using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarArchivoComprobanteACompraRecibida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "clave_archivo_comprobante",
                table: "compra_recibida",
                type: "character varying(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "fecha_carga_archivo_comprobante_utc",
                table: "compra_recibida",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nombre_archivo_comprobante",
                table: "compra_recibida",
                type: "character varying(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "tamano_bytes_archivo_comprobante",
                table: "compra_recibida",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_contenido_archivo_comprobante",
                table: "compra_recibida",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "clave_archivo_comprobante",
                table: "compra_recibida");

            migrationBuilder.DropColumn(
                name: "fecha_carga_archivo_comprobante_utc",
                table: "compra_recibida");

            migrationBuilder.DropColumn(
                name: "nombre_archivo_comprobante",
                table: "compra_recibida");

            migrationBuilder.DropColumn(
                name: "tamano_bytes_archivo_comprobante",
                table: "compra_recibida");

            migrationBuilder.DropColumn(
                name: "tipo_contenido_archivo_comprobante",
                table: "compra_recibida");
        }
    }
}
