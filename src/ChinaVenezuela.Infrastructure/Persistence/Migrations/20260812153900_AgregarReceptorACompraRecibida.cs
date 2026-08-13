using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReceptorACompraRecibida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "receptor_codigo_usuario",
                table: "compra_recibida",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_compra_recibida_receptor_codigo_usuario",
                table: "compra_recibida",
                column: "receptor_codigo_usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_compra_recibida_usuario_receptor_codigo_usuario",
                table: "compra_recibida",
                column: "receptor_codigo_usuario",
                principalTable: "usuario",
                principalColumn: "codigo_usuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_compra_recibida_usuario_receptor_codigo_usuario",
                table: "compra_recibida");

            migrationBuilder.DropIndex(
                name: "ix_compra_recibida_receptor_codigo_usuario",
                table: "compra_recibida");

            migrationBuilder.DropColumn(
                name: "receptor_codigo_usuario",
                table: "compra_recibida");
        }
    }
}
