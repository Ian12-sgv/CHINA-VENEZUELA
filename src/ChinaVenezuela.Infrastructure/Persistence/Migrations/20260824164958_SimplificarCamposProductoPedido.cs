using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SimplificarCamposProductoPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categoria",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "costo",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "fecha_pedido",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "nombre",
                table: "producto_pedido");

            migrationBuilder.DropColumn(
                name: "precio_detal",
                table: "producto_pedido");

            migrationBuilder.RenameColumn(
                name: "talla",
                table: "producto_pedido",
                newName: "curva_talla");

            migrationBuilder.RenameColumn(
                name: "referencia",
                table: "producto_pedido",
                newName: "referencia_asignada");

            migrationBuilder.RenameColumn(
                name: "marca",
                table: "producto_pedido",
                newName: "marca_producto");

            migrationBuilder.RenameColumn(
                name: "fabricante",
                table: "producto_pedido",
                newName: "fabrica");

            migrationBuilder.RenameColumn(
                name: "color",
                table: "producto_pedido",
                newName: "color_para_fabricar");

            migrationBuilder.RenameColumn(
                name: "codigo_barra",
                table: "producto_pedido",
                newName: "codigo_barra_asignado");

            migrationBuilder.RenameIndex(
                name: "IX_producto_pedido_codigo_barra",
                table: "producto_pedido",
                newName: "IX_producto_pedido_codigo_barra_asignado");

            migrationBuilder.AlterColumn<string>(
                name: "clave_almacenamiento",
                table: "producto_pedido_imagen",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "referencia_asignada",
                table: "producto_pedido",
                newName: "referencia");

            migrationBuilder.RenameColumn(
                name: "marca_producto",
                table: "producto_pedido",
                newName: "marca");

            migrationBuilder.RenameColumn(
                name: "fabrica",
                table: "producto_pedido",
                newName: "fabricante");

            migrationBuilder.RenameColumn(
                name: "curva_talla",
                table: "producto_pedido",
                newName: "talla");

            migrationBuilder.RenameColumn(
                name: "color_para_fabricar",
                table: "producto_pedido",
                newName: "color");

            migrationBuilder.RenameColumn(
                name: "codigo_barra_asignado",
                table: "producto_pedido",
                newName: "codigo_barra");

            migrationBuilder.RenameIndex(
                name: "IX_producto_pedido_codigo_barra_asignado",
                table: "producto_pedido",
                newName: "IX_producto_pedido_codigo_barra");

            migrationBuilder.AlterColumn<string>(
                name: "clave_almacenamiento",
                table: "producto_pedido_imagen",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "categoria",
                table: "producto_pedido",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "costo",
                table: "producto_pedido",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "fecha_pedido",
                table: "producto_pedido",
                type: "date",
                nullable: false,
                defaultValueSql: "CURRENT_DATE");

            migrationBuilder.AddColumn<string>(
                name: "nombre",
                table: "producto_pedido",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "precio_detal",
                table: "producto_pedido",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
