using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVerificacionCorreo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "correo_verificado",
                table: "usuario",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "token_verificacion_expira_utc",
                table: "usuario",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token_verificacion_hash",
                table: "usuario",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "usuario",
                keyColumn: "codigo_usuario",
                keyValue: "MARTHA",
                columns: new[] { "correo_verificado", "token_verificacion_expira_utc", "token_verificacion_hash" },
                values: new object[] { true, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "correo_verificado",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "token_verificacion_expira_utc",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "token_verificacion_hash",
                table: "usuario");
        }
    }
}
