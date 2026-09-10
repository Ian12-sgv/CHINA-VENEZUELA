using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFichasTecnicas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ficha_tecnica",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    categoria = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    linea = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    imagen_datos = table.Column<byte[]>(type: "bytea", nullable: true),
                    imagen_tipo_contenido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ficha_tecnica", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ficha_tecnica_atributo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ficha_tecnica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    atributo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    valor = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ficha_tecnica_atributo", x => x.id);
                    table.ForeignKey(
                        name: "FK_ficha_tecnica_atributo_ficha_tecnica_ficha_tecnica_id",
                        column: x => x.ficha_tecnica_id,
                        principalTable: "ficha_tecnica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ficha_tecnica_codigo",
                table: "ficha_tecnica",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ficha_tecnica_atributo_ficha_tecnica_id_atributo",
                table: "ficha_tecnica_atributo",
                columns: new[] { "ficha_tecnica_id", "atributo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ficha_tecnica_atributo");

            migrationBuilder.DropTable(
                name: "ficha_tecnica");
        }
    }
}
