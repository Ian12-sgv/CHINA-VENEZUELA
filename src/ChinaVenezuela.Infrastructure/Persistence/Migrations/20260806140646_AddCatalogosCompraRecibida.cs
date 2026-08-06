using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaVenezuela.Infrastructure.Persistence.Migrations
{
    public partial class AddCatalogosCompraRecibida : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contenedor_compartido",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_contenedor_compartido", x => x.id));

            migrationBuilder.CreateTable(
                name: "empresa",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_empresa", x => x.id));

            migrationBuilder.CreateTable(
                name: "marca_bulto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_marca_bulto", x => x.id));

            migrationBuilder.InsertData("contenedor_compartido", new[] { "id", "nombre" }, new object[,] {
                { new Guid("00000000-0000-0000-0000-000000000101"), "No compartido" },
                { new Guid("00000000-0000-0000-0000-000000000102"), "Compartido" }
            });
            migrationBuilder.InsertData("empresa", new[] { "id", "nombre" }, new object[,] {
                { new Guid("00000000-0000-0000-0000-000000000103"), "Sin especificar" }
            });

            migrationBuilder.Sql("""
                INSERT INTO empresa (id, nombre)
                SELECT md5(empresa)::uuid, empresa
                FROM compra_recibida
                WHERE btrim(empresa) <> ''
                GROUP BY empresa;
                """);
            migrationBuilder.Sql("""
                INSERT INTO marca_bulto (id, nombre)
                SELECT md5(marca_bultos)::uuid, marca_bultos
                FROM compra_recibida
                WHERE marca_bultos IS NOT NULL AND btrim(marca_bultos) <> ''
                GROUP BY marca_bultos;
                """);

            migrationBuilder.AddColumn<Guid>(name: "contenedor_compartido_id", table: "compra_recibida", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<Guid>(name: "empresa_id", table: "compra_recibida", type: "uuid", nullable: true);
            migrationBuilder.AddColumn<Guid>(name: "marca_bulto_id", table: "compra_recibida", type: "uuid", nullable: true);

            migrationBuilder.Sql("""
                UPDATE compra_recibida
                SET contenedor_compartido_id = CASE
                    WHEN contenedor_compartido THEN '00000000-0000-0000-0000-000000000102'::uuid
                    ELSE '00000000-0000-0000-0000-000000000101'::uuid
                END,
                empresa_id = CASE
                    WHEN btrim(empresa) = '' THEN '00000000-0000-0000-0000-000000000103'::uuid
                    ELSE md5(empresa)::uuid
                END,
                marca_bulto_id = CASE
                    WHEN marca_bultos IS NULL OR btrim(marca_bultos) = '' THEN NULL
                    ELSE md5(marca_bultos)::uuid
                END;
                """);

            migrationBuilder.AlterColumn<Guid>(name: "empresa_id", table: "compra_recibida", type: "uuid", nullable: false, oldClrType: typeof(Guid), oldType: "uuid", oldNullable: true);
            migrationBuilder.CreateIndex(name: "ix_compra_recibida_contenedor_compartido_id", table: "compra_recibida", column: "contenedor_compartido_id");
            migrationBuilder.CreateIndex(name: "ix_compra_recibida_empresa_id", table: "compra_recibida", column: "empresa_id");
            migrationBuilder.CreateIndex(name: "ix_compra_recibida_marca_bulto_id", table: "compra_recibida", column: "marca_bulto_id");
            migrationBuilder.CreateIndex(name: "IX_contenedor_compartido_nombre", table: "contenedor_compartido", column: "nombre", unique: true);
            migrationBuilder.CreateIndex(name: "IX_empresa_nombre", table: "empresa", column: "nombre", unique: true);
            migrationBuilder.CreateIndex(name: "IX_marca_bulto_nombre", table: "marca_bulto", column: "nombre", unique: true);
            migrationBuilder.AddForeignKey(name: "fk_compra_recibida_contenedor_compartido", table: "compra_recibida", column: "contenedor_compartido_id", principalTable: "contenedor_compartido", principalColumn: "id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "fk_compra_recibida_empresa", table: "compra_recibida", column: "empresa_id", principalTable: "empresa", principalColumn: "id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "fk_compra_recibida_marca_bulto", table: "compra_recibida", column: "marca_bulto_id", principalTable: "marca_bulto", principalColumn: "id", onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(name: "contenedor_compartido", table: "compra_recibida");
            migrationBuilder.DropColumn(name: "empresa", table: "compra_recibida");
            migrationBuilder.DropColumn(name: "marca_bultos", table: "compra_recibida");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(name: "contenedor_compartido", table: "compra_recibida", type: "boolean", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<string>(name: "empresa", table: "compra_recibida", type: "character varying(200)", maxLength: 200, nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "marca_bultos", table: "compra_recibida", type: "character varying(200)", maxLength: 200, nullable: true);
            migrationBuilder.Sql("""
                UPDATE compra_recibida c
                SET contenedor_compartido = c.contenedor_compartido_id = '00000000-0000-0000-0000-000000000102'::uuid,
                    empresa = e.nombre,
                    marca_bultos = m.nombre
                FROM empresa e
                LEFT JOIN marca_bulto m ON m.id = c.marca_bulto_id
                WHERE e.id = c.empresa_id;
                """);
            migrationBuilder.DropForeignKey("fk_compra_recibida_contenedor_compartido", "compra_recibida");
            migrationBuilder.DropForeignKey("fk_compra_recibida_empresa", "compra_recibida");
            migrationBuilder.DropForeignKey("fk_compra_recibida_marca_bulto", "compra_recibida");
            migrationBuilder.DropIndex("ix_compra_recibida_contenedor_compartido_id", "compra_recibida");
            migrationBuilder.DropIndex("ix_compra_recibida_empresa_id", "compra_recibida");
            migrationBuilder.DropIndex("ix_compra_recibida_marca_bulto_id", "compra_recibida");
            migrationBuilder.DropColumn("contenedor_compartido_id", "compra_recibida");
            migrationBuilder.DropColumn("empresa_id", "compra_recibida");
            migrationBuilder.DropColumn("marca_bulto_id", "compra_recibida");
            migrationBuilder.DropTable("contenedor_compartido");
            migrationBuilder.DropTable("empresa");
            migrationBuilder.DropTable("marca_bulto");
        }
    }
}