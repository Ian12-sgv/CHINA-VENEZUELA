using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRifYClasificacionEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "clasificacion",
                table: "empresa",
                type: "character varying(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rif",
                table: "empresa",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ux_empresa_rif",
                table: "empresa",
                column: "rif",
                unique: true,
                filter: "rif IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_empresa_rif",
                table: "empresa");

            migrationBuilder.DropColumn(
                name: "clasificacion",
                table: "empresa");

            migrationBuilder.DropColumn(
                name: "rif",
                table: "empresa");
        }
    }
}
