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

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818145420_AgregarModuloPedidos') THEN
    CREATE TABLE producto_pedido (
        id uuid NOT NULL,
        codigo_barra character varying(50) NOT NULL,
        referencia character varying(100) NOT NULL,
        nombre character varying(255) NOT NULL,
        marca character varying(100),
        categoria character varying(100) NOT NULL,
        talla character varying(50),
        color character varying(100),
        fabricante character varying(150),
        precio_detal numeric(12,2) NOT NULL,
        costo numeric(12,2) NOT NULL,
        activo boolean NOT NULL,
        creado_por_codigo_usuario character varying(50) NOT NULL,
        fecha_creacion_utc timestamp with time zone NOT NULL,
        CONSTRAINT "PK_producto_pedido" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818145420_AgregarModuloPedidos') THEN
    CREATE TABLE registro_precio_pedido (
        id uuid NOT NULL,
        codigo_barra character varying(50) NOT NULL,
        producto character varying(255) NOT NULL,
        sucursal character varying(150) NOT NULL,
        precio_sistema numeric(12,2) NOT NULL,
        precio_verificado numeric(12,2) NOT NULL,
        CONSTRAINT "PK_registro_precio_pedido" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818145420_AgregarModuloPedidos') THEN
    CREATE UNIQUE INDEX "IX_producto_pedido_codigo_barra" ON producto_pedido (codigo_barra);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818145420_AgregarModuloPedidos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260818145420_AgregarModuloPedidos', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818153955_AgregarFechaPedido') THEN
    ALTER TABLE producto_pedido ADD fecha_pedido date NOT NULL DEFAULT (CURRENT_DATE);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818153955_AgregarFechaPedido') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260818153955_AgregarFechaPedido', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818160016_AgregarEstadoEnvioPedido') THEN
    ALTER TABLE producto_pedido ADD enviado boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818160016_AgregarEstadoEnvioPedido') THEN
    ALTER TABLE producto_pedido ADD fecha_envio_utc timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818160016_AgregarEstadoEnvioPedido') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260818160016_AgregarEstadoEnvioPedido', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818162136_AgregarImagenProductoPedido') THEN
    CREATE TABLE producto_pedido_imagen (
        id uuid NOT NULL,
        producto_pedido_id uuid NOT NULL,
        clave_almacenamiento character varying(100) NOT NULL,
        nombre_original character varying(255) NOT NULL,
        tipo_contenido character varying(100) NOT NULL,
        tamano_bytes bigint NOT NULL,
        fecha_creacion_utc timestamp with time zone NOT NULL,
        fecha_actualizacion_utc timestamp with time zone,
        CONSTRAINT "PK_producto_pedido_imagen" PRIMARY KEY (id),
        CONSTRAINT "FK_producto_pedido_imagen_producto_pedido_producto_pedido_id" FOREIGN KEY (producto_pedido_id) REFERENCES producto_pedido (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818162136_AgregarImagenProductoPedido') THEN
    CREATE UNIQUE INDEX "IX_producto_pedido_imagen_producto_pedido_id" ON producto_pedido_imagen (producto_pedido_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260818162136_AgregarImagenProductoPedido') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260818162136_AgregarImagenProductoPedido', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260819153345_AgregarVerificacionCorreo') THEN
    ALTER TABLE usuario ADD correo_verificado boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260819153345_AgregarVerificacionCorreo') THEN
    ALTER TABLE usuario ADD token_verificacion_expira_utc timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260819153345_AgregarVerificacionCorreo') THEN
    ALTER TABLE usuario ADD token_verificacion_hash character varying(128);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260819153345_AgregarVerificacionCorreo') THEN
    UPDATE usuario SET correo_verificado = TRUE, token_verificacion_expira_utc = NULL, token_verificacion_hash = NULL
    WHERE codigo_usuario = 'MARTHA';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260819153345_AgregarVerificacionCorreo') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260819153345_AgregarVerificacionCorreo', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260819194348_AgregarEstadoComprobanteCompra') THEN
    ALTER TABLE compra_recibida ADD fecha_comprobante_enviado_utc timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260819194348_AgregarEstadoComprobanteCompra') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260819194348_AgregarEstadoComprobanteCompra', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824151851_AgregarDatosLogisticosProductoPedido') THEN
    ALTER TABLE producto_pedido ADD agente character varying(150);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824151851_AgregarDatosLogisticosProductoPedido') THEN
    ALTER TABLE producto_pedido ADD cantidad_unidades integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824151851_AgregarDatosLogisticosProductoPedido') THEN
    ALTER TABLE producto_pedido ADD composicion_tela character varying(255);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824151851_AgregarDatosLogisticosProductoPedido') THEN
    ALTER TABLE producto_pedido ADD marca_bulto character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824151851_AgregarDatosLogisticosProductoPedido') THEN
    ALTER TABLE producto_pedido ADD pack_por_caja integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824151851_AgregarDatosLogisticosProductoPedido') THEN
    ALTER TABLE producto_pedido ADD tipo_pedido character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824151851_AgregarDatosLogisticosProductoPedido') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260824151851_AgregarDatosLogisticosProductoPedido', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824153242_AgregarDosTiposImagenProductoPedido') THEN
    DROP INDEX "IX_producto_pedido_imagen_producto_pedido_id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824153242_AgregarDosTiposImagenProductoPedido') THEN
    ALTER TABLE producto_pedido_imagen ADD tipo character varying(30) NOT NULL DEFAULT 'ProductoTerminado';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824153242_AgregarDosTiposImagenProductoPedido') THEN
    CREATE UNIQUE INDEX "IX_producto_pedido_imagen_producto_pedido_id_tipo" ON producto_pedido_imagen (producto_pedido_id, tipo);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824153242_AgregarDosTiposImagenProductoPedido') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260824153242_AgregarDosTiposImagenProductoPedido', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido DROP COLUMN categoria;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido DROP COLUMN costo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido DROP COLUMN fecha_pedido;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido DROP COLUMN nombre;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido DROP COLUMN precio_detal;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido RENAME COLUMN talla TO curva_talla;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido RENAME COLUMN referencia TO referencia_asignada;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido RENAME COLUMN marca TO marca_producto;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido RENAME COLUMN fabricante TO fabrica;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido RENAME COLUMN color TO color_para_fabricar;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido RENAME COLUMN codigo_barra TO codigo_barra_asignado;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER INDEX "IX_producto_pedido_codigo_barra" RENAME TO "IX_producto_pedido_codigo_barra_asignado";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    ALTER TABLE producto_pedido_imagen ALTER COLUMN clave_almacenamiento TYPE character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260824164958_SimplificarCamposProductoPedido') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260824164958_SimplificarCamposProductoPedido', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN
    ALTER TABLE producto_pedido RENAME COLUMN tipo_pedido TO tipo_producto;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN
    CREATE TABLE agente_pedido (
        id uuid NOT NULL,
        nombre character varying(150) NOT NULL,
        CONSTRAINT "PK_agente_pedido" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN
    CREATE TABLE pedidos (
        id uuid NOT NULL,
        nombre character varying(150) NOT NULL,
        creado_por_codigo_usuario character varying(50) NOT NULL,
        fecha_creacion_utc timestamp with time zone NOT NULL,
        CONSTRAINT "PK_pedidos" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN
    CREATE TABLE pedidos_grupos (
        id uuid NOT NULL,
        pedido_id uuid NOT NULL,
        producto_pedido_id uuid NOT NULL,
        fecha_creacion_utc timestamp with time zone NOT NULL,
        CONSTRAINT "PK_pedidos_grupos" PRIMARY KEY (id),
        CONSTRAINT "FK_pedidos_grupos_pedidos_pedido_id" FOREIGN KEY (pedido_id) REFERENCES pedidos (id) ON DELETE CASCADE,
        CONSTRAINT "FK_pedidos_grupos_producto_pedido_producto_pedido_id" FOREIGN KEY (producto_pedido_id) REFERENCES producto_pedido (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN

                    INSERT INTO agente_pedido (id, nombre)
                    SELECT gen_random_uuid(), TRIM(p.agente)
                    FROM producto_pedido p
                    WHERE p.agente IS NOT NULL
                      AND BTRIM(p.agente) <> ''
                      AND NOT EXISTS (
                          SELECT 1 FROM agente_pedido a WHERE a.nombre = TRIM(p.agente)
                      );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN
    CREATE UNIQUE INDEX "IX_agente_pedido_nombre" ON agente_pedido (nombre);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN
    CREATE UNIQUE INDEX "IX_pedidos_nombre" ON pedidos (nombre);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN
    CREATE INDEX "IX_pedidos_grupos_pedido_id" ON pedidos_grupos (pedido_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN
    CREATE UNIQUE INDEX "IX_pedidos_grupos_producto_pedido_id" ON pedidos_grupos (producto_pedido_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825153917_AgregarGruposYAgentesPedidos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260825153917_AgregarGruposYAgentesPedidos', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825154432_AgregarAgrupacionYTipoProducto') THEN
    ALTER TABLE producto_pedido ALTER COLUMN tipo_producto TYPE character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260825154432_AgregarAgrupacionYTipoProducto') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260825154432_AgregarAgrupacionYTipoProducto', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260826145754_AgregarPrecioRmbTotalYCantidadDoz') THEN
    ALTER TABLE producto_pedido ADD cantidad_doz integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260826145754_AgregarPrecioRmbTotalYCantidadDoz') THEN
    ALTER TABLE producto_pedido ADD precio_rmb numeric(14,2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260826145754_AgregarPrecioRmbTotalYCantidadDoz') THEN
    ALTER TABLE producto_pedido ADD total_rmb numeric(14,2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260826145754_AgregarPrecioRmbTotalYCantidadDoz') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260826145754_AgregarPrecioRmbTotalYCantidadDoz', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260826151331_AgregarCantidadBultoACompraRecibida') THEN
    ALTER TABLE compra_recibida ADD cantidad_bulto integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260826151331_AgregarCantidadBultoACompraRecibida') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260826151331_AgregarCantidadBultoACompraRecibida', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260826152702_MoverCantidadBultoAPedidos') THEN
    ALTER TABLE compra_recibida DROP COLUMN cantidad_bulto;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260826152702_MoverCantidadBultoAPedidos') THEN
    ALTER TABLE producto_pedido ADD cantidad_bulto integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260826152702_MoverCantidadBultoAPedidos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260826152702_MoverCantidadBultoAPedidos', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260827152421_AgregarFechasRegistroEInicioFabricacionPedido') THEN
    ALTER TABLE producto_pedido ADD fecha_inicio_fabricacion date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260827152421_AgregarFechasRegistroEInicioFabricacionPedido') THEN
    ALTER TABLE producto_pedido ADD fecha_registro_pedido date NOT NULL DEFAULT (CURRENT_DATE);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260827152421_AgregarFechasRegistroEInicioFabricacionPedido') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260827152421_AgregarFechasRegistroEInicioFabricacionPedido', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260828192530_AgregarMultiplesBultosPorProducto') THEN
    CREATE TABLE producto_pedido_bulto (
        id uuid NOT NULL,
        producto_pedido_id uuid NOT NULL,
        marca_bulto_id uuid NOT NULL,
        cantidad integer NOT NULL,
        CONSTRAINT "PK_producto_pedido_bulto" PRIMARY KEY (id),
        CONSTRAINT "FK_producto_pedido_bulto_marca_bulto_marca_bulto_id" FOREIGN KEY (marca_bulto_id) REFERENCES marca_bulto (id) ON DELETE RESTRICT,
        CONSTRAINT "FK_producto_pedido_bulto_producto_pedido_producto_pedido_id" FOREIGN KEY (producto_pedido_id) REFERENCES producto_pedido (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260828192530_AgregarMultiplesBultosPorProducto') THEN
    CREATE INDEX "IX_producto_pedido_bulto_marca_bulto_id" ON producto_pedido_bulto (marca_bulto_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260828192530_AgregarMultiplesBultosPorProducto') THEN
    CREATE UNIQUE INDEX "IX_producto_pedido_bulto_producto_pedido_id_marca_bulto_id" ON producto_pedido_bulto (producto_pedido_id, marca_bulto_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260828192530_AgregarMultiplesBultosPorProducto') THEN
    INSERT INTO marca_bulto (id, nombre)
    SELECT md5('legacy-marca-bulto:' || lower(trim(p.marca_bulto)))::uuid, trim(p.marca_bulto)
    FROM producto_pedido AS p
    WHERE p.marca_bulto IS NOT NULL
      AND btrim(p.marca_bulto) <> ''
      AND p.cantidad_bulto IS NOT NULL
      AND p.cantidad_bulto > 0
      AND NOT EXISTS (
          SELECT 1
          FROM marca_bulto AS m
          WHERE lower(m.nombre) = lower(trim(p.marca_bulto))
      );

    INSERT INTO producto_pedido_bulto (id, producto_pedido_id, marca_bulto_id, cantidad)
    SELECT md5('legacy-producto-pedido-bulto:' || p.id::text || ':' || m.id::text)::uuid,
           p.id,
           m.id,
           p.cantidad_bulto
    FROM producto_pedido AS p
    INNER JOIN marca_bulto AS m
        ON lower(m.nombre) = lower(trim(p.marca_bulto))
    WHERE p.marca_bulto IS NOT NULL
      AND btrim(p.marca_bulto) <> ''
      AND p.cantidad_bulto IS NOT NULL
      AND p.cantidad_bulto > 0
      AND NOT EXISTS (
          SELECT 1
          FROM producto_pedido_bulto AS pb
          WHERE pb.producto_pedido_id = p.id
            AND pb.marca_bulto_id = m.id
      );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260828192530_AgregarMultiplesBultosPorProducto') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260828192530_AgregarMultiplesBultosPorProducto', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260831201211_AgregarStatusACompraRecibida') THEN
    ALTER TABLE compra_recibida ADD status character varying(20) NOT NULL DEFAULT 'En proceso';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260831201211_AgregarStatusACompraRecibida') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260831201211_AgregarStatusACompraRecibida', '10.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260903210249_AgregarArchivoComprobanteACompraRecibida') THEN
    ALTER TABLE compra_recibida ADD clave_archivo_comprobante character varying(260);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260903210249_AgregarArchivoComprobanteACompraRecibida') THEN
    ALTER TABLE compra_recibida ADD fecha_carga_archivo_comprobante_utc timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260903210249_AgregarArchivoComprobanteACompraRecibida') THEN
    ALTER TABLE compra_recibida ADD nombre_archivo_comprobante character varying(260);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260903210249_AgregarArchivoComprobanteACompraRecibida') THEN
    ALTER TABLE compra_recibida ADD tamano_bytes_archivo_comprobante bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260903210249_AgregarArchivoComprobanteACompraRecibida') THEN
    ALTER TABLE compra_recibida ADD tipo_contenido_archivo_comprobante character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260903210249_AgregarArchivoComprobanteACompraRecibida') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260903210249_AgregarArchivoComprobanteACompraRecibida', '10.0.0');
    END IF;
END $EF$;
COMMIT;

