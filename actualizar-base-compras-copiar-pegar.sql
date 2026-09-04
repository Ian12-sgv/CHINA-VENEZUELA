-- CHINA - VENEZUELA | Actualización segura de Compras
-- Ejecutar en PostgreSQL sobre la base china_venezuela.
-- No elimina registros. Puede ejecutarse más de una vez.

BEGIN;

ALTER TABLE public.compra_recibida
    ADD COLUMN IF NOT EXISTS status character varying(20) NOT NULL DEFAULT 'En proceso';

UPDATE public.compra_recibida
SET status = 'En proceso'
WHERE status IS NULL;

ALTER TABLE public.compra_recibida
    ALTER COLUMN status SET DEFAULT 'En proceso',
    ALTER COLUMN status SET NOT NULL;

ALTER TABLE public.compra_recibida
    ADD COLUMN IF NOT EXISTS clave_archivo_comprobante character varying(260),
    ADD COLUMN IF NOT EXISTS fecha_carga_archivo_comprobante_utc timestamp with time zone,
    ADD COLUMN IF NOT EXISTS nombre_archivo_comprobante character varying(260),
    ADD COLUMN IF NOT EXISTS tamano_bytes_archivo_comprobante bigint,
    ADD COLUMN IF NOT EXISTS tipo_contenido_archivo_comprobante character varying(100);

INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES
    ('20260831201211_AgregarStatusACompraRecibida', '10.0.0'),
    ('20260903210249_AgregarArchivoComprobanteACompraRecibida', '10.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;

COMMIT;
