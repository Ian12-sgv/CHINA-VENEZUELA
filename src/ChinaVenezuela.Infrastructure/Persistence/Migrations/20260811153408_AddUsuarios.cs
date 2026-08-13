using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    codigo_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    contrasena_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.codigo_usuario);
                });

            migrationBuilder.CreateTable(
                name: "grupo_usuario",
                columns: table => new
                {
                    codigo_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre_grupo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupo_usuario", x => new { x.codigo_usuario, x.nombre_grupo });
                    table.ForeignKey(
                        name: "FK_grupo_usuario_usuario_codigo_usuario",
                        column: x => x.codigo_usuario,
                        principalTable: "usuario",
                        principalColumn: "codigo_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "usuario",
                columns: new[] { "codigo_usuario", "contrasena_hash", "nombre", "status" },
                values: new object[] { "MARTHA", "PBKDF2-SHA256$600000$/ArqC9UVvmvFUXd6R3AnLw==$7LqpJF/sNGKbmYt60NGHe4RBjujsh04P8DkWFUvBVWI=", "Martha", true });

            migrationBuilder.InsertData(
                table: "grupo_usuario",
                columns: new[] { "codigo_usuario", "nombre_grupo" },
                values: new object[] { "MARTHA", "Administradores" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grupo_usuario");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
