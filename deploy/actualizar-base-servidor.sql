CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260805160730_InitialCompraRecibida') THEN
    CREATE TABLE compra_recibida (
        id uuid NOT NULL,
        contenedor_compartido boolean NOT NULL DEFAULT FALSE,
        nombre_contenedor character varying(200) NOT NULL,
        numero_contenedor character varying(100) NOT NULL,
        empresa character varying(200) NOT NULL,
        descripcion character varying(2000),
        fecha_salida date NOT NULL,
        fecha_llegada date,
        aduana character varying(200),
        puerto_llegada character varying(200) NOT NULL,
        marca_bultos character varying(200),
        fecha_creacion_utc timestamp with time zone NOT NULL,
        fecha_actualizacion_utc timestamp with time zone,
        CONSTRAINT "PK_compra_recibida" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260805160730_InitialCompraRecibida') THEN
    CREATE TABLE registro_auditoria (
        id uuid NOT NULL,
        tipo_entidad character varying(100) NOT NULL,
        entidad_id uuid NOT NULL,
        accion character varying(30) NOT NULL,
        valores_antes_json jsonb,
        valores_despues_json jsonb,
        fecha_utc timestamp with time zone NOT NULL,
        CONSTRAINT "PK_registro_auditoria" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260805160730_InitialCompraRecibida') THEN
    CREATE INDEX ix_compra_recibida_numero_contenedor ON compra_recibida (numero_contenedor);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260805160730_InitialCompraRecibida') THEN
    CREATE INDEX ix_registro_auditoria_tipo_entidad_entidad_id ON registro_auditoria (tipo_entidad, entidad_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260805160730_InitialCompraRecibida') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260805160730_InitialCompraRecibida', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    CREATE TABLE contenedor_compartido (
        id uuid NOT NULL,
        nombre character varying(200) NOT NULL,
        CONSTRAINT "PK_contenedor_compartido" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    CREATE TABLE empresa (
        id uuid NOT NULL,
        nombre character varying(200) NOT NULL,
        CONSTRAINT "PK_empresa" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    CREATE TABLE marca_bulto (
        id uuid NOT NULL,
        nombre character varying(200) NOT NULL,
        CONSTRAINT "PK_marca_bulto" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    INSERT INTO contenedor_compartido (id, nombre)
    VALUES ('00000000-0000-0000-0000-000000000101', 'No compartido');
    INSERT INTO contenedor_compartido (id, nombre)
    VALUES ('00000000-0000-0000-0000-000000000102', 'Compartido');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    INSERT INTO empresa (id, nombre)
    VALUES ('00000000-0000-0000-0000-000000000103', 'Sin especificar');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    INSERT INTO empresa (id, nombre)
    SELECT md5(empresa)::uuid, empresa
    FROM compra_recibida
    WHERE btrim(empresa) <> ''
    GROUP BY empresa;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    INSERT INTO marca_bulto (id, nombre)
    SELECT md5(marca_bultos)::uuid, marca_bultos
    FROM compra_recibida
    WHERE marca_bultos IS NOT NULL AND btrim(marca_bultos) <> ''
    GROUP BY marca_bultos;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida ADD contenedor_compartido_id uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida ADD empresa_id uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida ADD marca_bulto_id uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida ALTER COLUMN empresa_id SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    CREATE INDEX ix_compra_recibida_contenedor_compartido_id ON compra_recibida (contenedor_compartido_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    CREATE INDEX ix_compra_recibida_empresa_id ON compra_recibida (empresa_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    CREATE INDEX ix_compra_recibida_marca_bulto_id ON compra_recibida (marca_bulto_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    CREATE UNIQUE INDEX "IX_contenedor_compartido_nombre" ON contenedor_compartido (nombre);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    CREATE UNIQUE INDEX "IX_empresa_nombre" ON empresa (nombre);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    CREATE UNIQUE INDEX "IX_marca_bulto_nombre" ON marca_bulto (nombre);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida ADD CONSTRAINT fk_compra_recibida_contenedor_compartido FOREIGN KEY (contenedor_compartido_id) REFERENCES contenedor_compartido (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida ADD CONSTRAINT fk_compra_recibida_empresa FOREIGN KEY (empresa_id) REFERENCES empresa (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida ADD CONSTRAINT fk_compra_recibida_marca_bulto FOREIGN KEY (marca_bulto_id) REFERENCES marca_bulto (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida DROP COLUMN contenedor_compartido;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida DROP COLUMN empresa;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    ALTER TABLE compra_recibida DROP COLUMN marca_bultos;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260806140646_AddCatalogosCompraRecibida') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260806140646_AddCatalogosCompraRecibida', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811153408_AddUsuarios') THEN
    CREATE TABLE usuario (
        codigo_usuario character varying(50) NOT NULL,
        nombre character varying(200) NOT NULL,
        contrasena_hash character varying(500) NOT NULL,
        status boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_usuario" PRIMARY KEY (codigo_usuario)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811153408_AddUsuarios') THEN
    CREATE TABLE grupo_usuario (
        codigo_usuario character varying(50) NOT NULL,
        nombre_grupo character varying(100) NOT NULL,
        CONSTRAINT "PK_grupo_usuario" PRIMARY KEY (codigo_usuario, nombre_grupo),
        CONSTRAINT "FK_grupo_usuario_usuario_codigo_usuario" FOREIGN KEY (codigo_usuario) REFERENCES usuario (codigo_usuario) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811153408_AddUsuarios') THEN
    INSERT INTO usuario (codigo_usuario, contrasena_hash, nombre, status)
    VALUES ('MARTHA', 'PBKDF2-SHA256$600000$/ArqC9UVvmvFUXd6R3AnLw==$7LqpJF/sNGKbmYt60NGHe4RBjujsh04P8DkWFUvBVWI=', 'Martha', TRUE);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811153408_AddUsuarios') THEN
    INSERT INTO grupo_usuario (codigo_usuario, nombre_grupo)
    VALUES ('MARTHA', 'Administradores');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811153408_AddUsuarios') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260811153408_AddUsuarios', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812130124_AgregarCatalogoGrupos') THEN
    CREATE TABLE grupo (
        nombre character varying(100) NOT NULL,
        CONSTRAINT "PK_grupo" PRIMARY KEY (nombre)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812130124_AgregarCatalogoGrupos') THEN
    INSERT INTO grupo (nombre)
    VALUES ('Prueba');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812130124_AgregarCatalogoGrupos') THEN
    CREATE UNIQUE INDEX ux_usuario_nombre ON usuario (nombre);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812130124_AgregarCatalogoGrupos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260812130124_AgregarCatalogoGrupos', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812144531_AgregarRifYClasificacionEmpresa') THEN
    ALTER TABLE empresa ADD clasificacion character varying(12);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812144531_AgregarRifYClasificacionEmpresa') THEN
    ALTER TABLE empresa ADD rif character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812144531_AgregarRifYClasificacionEmpresa') THEN
    CREATE UNIQUE INDEX ux_empresa_rif ON empresa (rif) WHERE rif IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812144531_AgregarRifYClasificacionEmpresa') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260812144531_AgregarRifYClasificacionEmpresa', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812151833_AgregarCatalogosAduanaYPuertoLlegada') THEN
    CREATE TABLE aduana (
        id uuid NOT NULL,
        nombre character varying(200) NOT NULL,
        CONSTRAINT "PK_aduana" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812151833_AgregarCatalogosAduanaYPuertoLlegada') THEN
    CREATE TABLE puerto_llegada (
        id uuid NOT NULL,
        nombre character varying(200) NOT NULL,
        CONSTRAINT "PK_puerto_llegada" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812151833_AgregarCatalogosAduanaYPuertoLlegada') THEN
    CREATE UNIQUE INDEX "IX_aduana_nombre" ON aduana (nombre);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812151833_AgregarCatalogosAduanaYPuertoLlegada') THEN
    CREATE UNIQUE INDEX "IX_puerto_llegada_nombre" ON puerto_llegada (nombre);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812151833_AgregarCatalogosAduanaYPuertoLlegada') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260812151833_AgregarCatalogosAduanaYPuertoLlegada', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812153900_AgregarReceptorACompraRecibida') THEN
    ALTER TABLE compra_recibida ADD receptor_codigo_usuario character varying(50);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812153900_AgregarReceptorACompraRecibida') THEN
    CREATE INDEX ix_compra_recibida_receptor_codigo_usuario ON compra_recibida (receptor_codigo_usuario);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812153900_AgregarReceptorACompraRecibida') THEN
    ALTER TABLE compra_recibida ADD CONSTRAINT "FK_compra_recibida_usuario_receptor_codigo_usuario" FOREIGN KEY (receptor_codigo_usuario) REFERENCES usuario (codigo_usuario) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812153900_AgregarReceptorACompraRecibida') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260812153900_AgregarReceptorACompraRecibida', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812160038_AgregarCorreoAUsuario') THEN
    ALTER TABLE usuario ADD correo character varying(254);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812160038_AgregarCorreoAUsuario') THEN
    UPDATE usuario SET correo = NULL
    WHERE codigo_usuario = 'MARTHA';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812160038_AgregarCorreoAUsuario') THEN
    CREATE UNIQUE INDEX ux_usuario_correo ON usuario (correo) WHERE correo IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260812160038_AgregarCorreoAUsuario') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260812160038_AgregarCorreoAUsuario', '10.0.0');
    END IF;
END $EF$;
COMMIT;

