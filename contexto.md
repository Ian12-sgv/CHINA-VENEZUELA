# Contexto técnico — Sistema China → Venezuela

> Referencia para desarrollo, QA, despliegue y soporte.
> Última actualización: **10 de septiembre de 2026**.
> **Seguridad:** no guardar contraseñas, cadenas de conexión completas, JWT ni claves de Resend en este archivo o en Git.

## 1. Ficha técnica

| Elemento | Estado actual |
| --- | --- |
| Propósito | Gestionar compras recibidas, pedidos/productos, catálogos operativos, comprobantes y rastreo. |
| Frontend | React 19, TypeScript, Vite 8, TanStack Query, SignalR, jsPDF y Font Awesome. |
| Backend | ASP.NET Core 10, C#, Entity Framework Core 10 y OpenAPI. |
| Base de datos | PostgreSQL, esquema `public`, columnas en `snake_case`. |
| Seguridad | JWT, hash PBKDF2, CORS limitado y HTTPS por Caddy. |
| Frontend productivo | `https://tracking-china.netlify.app` |
| API productiva | `https://api-china.apipalacio.com` |
| Proxy | Caddy dirige la API pública a `127.0.0.1:5081`. |
| Repositorio de producción | `Ian12-sgv/CHINA-VENEZUELA`, rama `main`. |

## 2. Arquitectura

```text
Navegador
  ├─ Netlify ─► React/Vite
  └─ api-china.apipalacio.com ─► Caddy ─► ASP.NET Core :5081
                                                   ├─ PostgreSQL
                                                   ├─ archivos locales protegidos
                                                   └─ Resend
```

| Capa | Responsabilidad |
| --- | --- |
| `ChinaVenezuela.Api` | Controllers, JWT, CORS, OpenAPI, SignalR, correo y carga de archivos. |
| `ChinaVenezuela.Application` | Casos de uso, contratos/DTO, validaciones e interfaces. |
| `ChinaVenezuela.Domain` | Entidades y reglas de negocio. |
| `ChinaVenezuela.Infrastructure` | EF Core, Npgsql, repositorios, migraciones y almacenamiento local. |
| `frontend` | Interfaz React, cliente API, exportación y estado visual. |

## 3. Estructura relevante

```text
China/
├── frontend/src/
│   ├── pages/                 # Compras, Pedidos, Rastreo, Ficha técnica, Usuarios, Grupos y Catálogos
│   ├── components/, hooks/, utils/
│   ├── api.ts, types.ts, App.tsx
│   └── index.css
├── src/
│   ├── ChinaVenezuela.Api/    # Auth, Controllers, Comprobantes, Hubs
│   ├── ChinaVenezuela.Application/
│   ├── ChinaVenezuela.Domain/
│   └── ChinaVenezuela.Infrastructure/Persistence/Migrations/
├── tests/ChinaVenezuela.Application.Tests/
├── qa/errores/                # Reportes QA/testing
├── skills/                     # Definiciones de agentes y prácticas
├── datos/imagenes/             # Ignorado por Git
├── backend-publicar*/          # Paquetes Release locales; ignorados por Git
├── actualizar-base-fichas-tecnicas.sql     # Script idempotente vigente (31 migraciones)
├── actualizar-base-compras.sql             # Snapshots históricos de scripts anteriores
├── actualizar-base-compras-copiar-pegar.sql
├── actualizar-base-status-compras.sql
├── actualizar-base-multiples-bultos.sql
├── guia.md
├── contexto.md
└── ChinaVenezuela.slnx
```

No subir secretos, `.env`, logs, imágenes, RAR ni las carpetas `backend-publicar*`.

## 4. Módulos y reglas implementadas

### Usuarios, grupos y campos operativos

- Inicio de sesión JWT; contraseñas almacenadas con PBKDF2.
- `MS` y `SIS`: administración completa. `oficina`: Compras y Campos operativos. `Pedidos`: Pedidos, Rastreo y Campos operativos.
- Políticas API: `GestionGrupos`, `AccesoCompras`, `AccesoPedidos` y `AccesoOperativo`.
- Campos operativos: contenedor compartido, empresas (RIF único y clasificación Oriente/Occidente/Aliada), marca de bulto, aduana, puerto de llegada y agentes.

### Compras recibidas

- Registro, edición y eliminación de recibos pendientes.
- Historial con buscador, filtro por fecha de llegada y actualización SignalR.
- Status: **En proceso**, **Aprobado** y **Sin terminar**.
- Carga, consulta y eliminación de archivo de comprobante: PDF, JPEG, PNG o WebP, hasta 15 MB.
- Envío por Resend al receptor, con copia al usuario creador.
- El correo adjunta Excel y PDF generados; también adjunta el archivo de comprobante cargado si existe.
- Después de enviar comprobante, el recibo no se puede editar ni eliminar.

### Pedidos y catálogo de productos

- Grupos de pedido con correlativo automático: `001`, `002`, etc.
- Productos nuevos/repetidos con agente, fechas, fábrica, composición, color, marca, curva, pack, unidades, referencia y código de barras.
- Precio/total en RMB/CNY (`¥`) y cantidad DOZ.
- Dos imágenes por producto: fábrica y producto terminado; JPEG, PNG o WebP, máximo 15 MB.
- Varias marcas de bulto por subpedido, cada una con cantidad propia.
- El catálogo y exportaciones Excel/PDF muestran la marca de bulto como encabezado y su cantidad debajo.
- Duplicar un subpedido solicita código de barras nuevo y único; conserva grupo/datos y copia ambas imágenes como archivos independientes.
- Un producto enviado queda bloqueado contra edición y eliminación.
- Al escribir la **Referencia asignada** y presionar Enter, busca una ficha técnica con esa misma referencia y, si existe, la muestra vinculada (código, categoría y línea) en el formulario.
- El catálogo muestra una columna **Ficha técnica** por subpedido: busca por referencia y solo pinta el código si hay coincidencia (no es una relación real en base de datos).

### Ficha técnica

- Catálogo independiente de fichas técnicas de producto, visible para `Compras` y `Pedidos`.
- Código y referencia únicos; categoría, línea y estado (**Activo**/**Inactivo**) obligatorios.
- Imagen opcional JPEG/PNG/WebP, máximo 2 MB — a diferencia de Compras/Pedidos, **se guarda como `bytea` dentro de PostgreSQL**, no en disco.
- Atributos personalizados por ficha (atributo/valor únicos por ficha, observación opcional, y campos opcionales de composición de tela, color, marca y curva de talla propios de cada atributo).
- Eliminar una ficha borra en cascada sus atributos.
- Se vincula con Pedidos únicamente por coincidencia de texto en `referencia` (ver arriba); no hay clave foránea entre `producto_pedido` y `ficha_tecnica`.

### Rastreo

- Disponible para `Pedidos` y administradores.
- Línea de tiempo: Lista creada, Pedido confirmado, Fábrica recibió, En producción, Producción lista, Control de calidad, Validado y enviado, En tránsito, Entregado.
- Estado actual: **modo de prueba**. Clic en una etapa para marcarla; clic en la etapa verde para desmarcar solo esa marca. No escribe en la base de datos.
- Para producción real se debe definir un estado persistente o una tabla de eventos por producto/pedido.

## 5. Modelo de datos

| Tabla | Responsabilidad |
| --- | --- |
| `usuario`, `grupo`, `grupo_usuario` | Identidad, credenciales y autorización. |
| `compra_recibida` | Recibo, fechas, receptor, status, comprobante y archivo adjunto. |
| `empresa`, `contenedor_compartido`, `marca_bulto`, `aduana`, `puerto_llegada` | Catálogos de Compras. |
| `agente_pedido` | Catálogo de agentes. |
| `pedidos`, `pedidos_grupos` | Agrupación de pedidos. |
| `producto_pedido` | Datos comerciales y logísticos de cada subpedido. |
| `producto_pedido_bulto` | Relación producto–marca de bulto–cantidad. |
| `producto_pedido_imagen` | Metadatos de imágenes de fábrica y producto terminado. |
| `registro_auditoria`, `registro_precio_pedido` | Auditoría e histórico. |
| `ficha_tecnica` | Código/referencia únicos, categoría, línea, estado e imagen (`bytea`). |
| `ficha_tecnica_atributo` | Atributos personalizados por ficha; único por `ficha_tecnica_id` + `atributo`. |
| `__EFMigrationsHistory` | Migraciones EF Core aplicadas. |

```text
usuario ──< grupo_usuario >── grupo
empresa/contenedor/marca/puerto/aduana ──< compra_recibida
pedidos ──< pedidos_grupos >── producto_pedido ──< producto_pedido_imagen
                                           └──< producto_pedido_bulto >── marca_bulto
ficha_tecnica ──< ficha_tecnica_atributo         (sin FK; se enlaza a producto_pedido solo por texto de "referencia")
```

## 6. API y tiempo real

OpenAPI: `https://api-china.apipalacio.com/openapi/v1.json`

| Área | Rutas relevantes |
| --- | --- |
| Auth | `POST /api/auth/iniciar-sesion`, `GET /api/cuenta` |
| Compras | CRUD `/api/compras-recibidas`, `PUT /{id}/status` |
| Comprobantes | `POST /{id}/comprobante/enviar`; `PUT/GET/DELETE /{id}/archivo-comprobante` |
| Catálogos | CRUD de empresas, marcas, contenedores, aduanas y puertos. |
| Agentes | CRUD `/api/agentes` |
| Pedidos | Productos, grupos, envío, duplicación e imágenes en `/api/pedidos/...` |
| Ficha técnica | CRUD `/api/fichas-tecnicas`, `GET /por-referencia/{referencia}`, atributos e imagen en `/{id}/atributos` y `/{id}/imagen`. |
| Tiempo real | Hub autenticado `/hub/actualizaciones` |

## 7. Configuración

Configurar fuera del repositorio:

```text
ConnectionStrings__PostgreSql
Jwt__Key
Resend__ApiKey
Resend__RemitenteCorreo
Resend__RemitenteNombre
ASPNETCORE_URLS=http://127.0.0.1:5081
```

En Netlify:

```text
VITE_API_URL=https://api-china.apipalacio.com/api
```

CORS permite `https://tracking-china.netlify.app`. Si cambia el dominio frontend, modificar la política en `Program.cs`, publicar y reiniciar la API.

## 8. Ejecución, compilación y QA

```powershell
# Backend, desde la raíz (definir primero PostgreSQL y Jwt__Key)
dotnet run --project src\ChinaVenezuela.Api

# Frontend, en otra consola
cd frontend
npm install
npm run dev
```

| Servicio | Dirección local |
| --- | --- |
| Frontend | `http://127.0.0.1:5173` |
| API | `http://127.0.0.1:5081` |
| OpenAPI | `http://127.0.0.1:5081/openapi/v1.json` |

Antes de publicar:

```powershell
dotnet build ChinaVenezuela.slnx -c Release
dotnet test ChinaVenezuela.slnx -c Release --no-build
cd frontend
npm run build
```

`npm run build` ejecuta TypeScript. Un import o variable sin uso puede detener el deploy de Netlify durante **Building**.

El agente `qa-tester` valida compilación, pruebas, API, permisos, archivos, reportes y migraciones; los hallazgos se guardan en `qa/errores/`. El agente `arreglo` debe leer este archivo, `guia.md` y el reporte antes de corregir; un error se cierra únicamente con evidencia de prueba satisfactoria.

## 9. Base de datos y migraciones

| Migración | Cambio |
| --- | --- |
| `20260827152421_AgregarFechasRegistroEInicioFabricacionPedido` | Fechas de registro e inicio de fabricación. |
| `20260828192530_AgregarMultiplesBultosPorProducto` | Tabla `producto_pedido_bulto` y migración de datos previos. |
| `20260831201211_AgregarStatusACompraRecibida` | Campo `status`, valor inicial `En proceso`. |
| `20260903210249_AgregarArchivoComprobanteACompraRecibida` | Metadatos del archivo adjunto de comprobante. |
| `20260909184007_AgregarFichasTecnicas` | Tablas `ficha_tecnica` y `ficha_tecnica_atributo`. |
| `20260909210444_AgregarReferenciaFichaTecnica` | Columna `referencia` en `ficha_tecnica`. |
| `20260909211240_AgregarDetallesTecnicosFicha` | Composición de tela, color, marca y curva de talla en `ficha_tecnica`. |
| `20260909212701_AgregarDetallesAtributoFicha` | Mismos campos técnicos, ahora también por atributo en `ficha_tecnica_atributo`. |
| `20260910124445_HacerUnicaReferenciaFichaTecnica` | Índice único sobre `referencia` en `ficha_tecnica`. |

Las migraciones de Compras (`AgregarStatusACompraRecibida` y `AgregarArchivoComprobanteACompraRecibida`) son obligatorias para la versión actual del módulo. Si no existen en el servidor, el frontend puede mostrar un aviso genérico de conexión aunque el origen real sea un error 500 de BD.

`actualizar-base-fichas-tecnicas.sql` es el script idempotente **más reciente** (generado el 10/09/2026 con `dotnet ef migrations script --idempotent`, sin argumentos): incluye las 31 migraciones completas, desde la inicial hasta las 5 de Ficha técnica. Al ser idempotente, se puede ejecutar sobre cualquier base que ya tenga aplicada una parte del historial: solo crea lo que falte. Reemplaza a `actualizar-base-compras.sql`/`actualizar-base-compras-copiar-pegar.sql` como referencia vigente; esos dos quedan como snapshot histórico.

Flujo seguro de producción:

1. Respaldar PostgreSQL.
2. Detener API si se actualizará el ejecutable/estructura.
3. Ejecutar `actualizar-base-fichas-tecnicas.sql` (o el script compacto vigente en ese momento).
4. Confirmar migraciones en `__EFMigrationsHistory`.
5. Iniciar backend y probar Compras (y Ficha técnica, una vez desplegado su código).

```powershell
$pgBin = 'C:\Program Files\PostgreSQL\18\bin'
$backupDir = 'C:\Users\Administrator\Documents\servicios IAN\respaldos-china'
$fecha = Get-Date -Format 'yyyyMMdd-HHmmss'
New-Item -ItemType Directory -Force -Path $backupDir
& "$pgBin\pg_dump.exe" -h localhost -p 5432 -U postgres -W `
  -F c -f "$backupDir\china_venezuela_$fecha.dump" china_venezuela

$scriptSql = 'C:\Users\Administrator\Documents\servicios IAN\actualizar-base-fichas-tecnicas.sql'
& "$pgBin\psql.exe" -h localhost -p 5432 -U postgres -d china_venezuela -W `
  -v ON_ERROR_STOP=1 -f $scriptSql
```

El script compacto usa `IF NOT EXISTS` y no elimina registros, pero el respaldo sigue siendo obligatorio.

```sql
SELECT "MigrationId"
FROM "__EFMigrationsHistory"
WHERE "MigrationId" IN (
  '20260831201211_AgregarStatusACompraRecibida',
  '20260903210249_AgregarArchivoComprobanteACompraRecibida'
)
ORDER BY "MigrationId";
```

## 10. Despliegue

### Frontend

| Configuración Netlify | Valor |
| --- | --- |
| Repositorio | `Ian12-sgv/CHINA-VENEZUELA` |
| Rama | `main` |
| Base directory | `frontend` |
| Build | `npm run build` |
| Publish | `dist` |
| Variable | `VITE_API_URL=https://api-china.apipalacio.com/api` |

Un push a `main` dispara Netlify. Si falla en compilación, reproducir primero `npm run build` localmente.

### Backend

```powershell
dotnet publish src\ChinaVenezuela.Api\ChinaVenezuela.Api.csproj -c Release -o backend-publicar-compras
```

Copiar el contenido Release al servidor sin sobrescribir secretos ni carpetas persistentes de archivos. Caddy debe mantener:

```caddyfile
api-china.apipalacio.com {
    encode zstd gzip
    reverse_proxy 127.0.0.1:5081
}
```

Los paquetes `backend-publicar*` son locales/ignorados por Git; el código fuente y los scripts SQL sí se versionan.

Para detener solo procesos registrados por los scripts del servidor:

```powershell
$pidDir = 'C:\Users\Administrator\Documents\servicios IAN\pids'
Get-ChildItem -LiteralPath $pidDir -Filter '*.pid' -File -ErrorAction SilentlyContinue |
  ForEach-Object {
    $processId = [int](Get-Content -LiteralPath $_.FullName -Raw)
    Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
    Remove-Item -LiteralPath $_.FullName -Force -ErrorAction SilentlyContinue
  }
```

## 11. Git, documentación y próximos pasos

| Repositorio | Uso |
| --- | --- |
| `Ian12-sgv/CHINA-VENEZUELA` | Producción Netlify; validar antes de actualizar `main`. |
| `PDB-sis/tracking` | Colaborativo; `main` está protegida y requiere Pull Request. |

- Cada función nueva debe actualizar `guia.md`.
- Si cambia arquitectura, datos, seguridad o despliegue, actualizar además `contexto.md`.
- Respaldar BD y directorio de archivos antes de cambios productivos.
- `docker-compose.yml` ya usa PostgreSQL (`postgres:16-alpine`) y `ConnectionStrings__PostgreSql`, con `Jwt__Key`/`POSTGRES_PASSWORD` por variables de entorno (`.env`, ignorado por Git). Sigue sin ser el método de despliegue productivo actual (ver sección 10).
- Pendiente: persistir las etapas reales de Rastreo, monitorear logs/disco y programar respaldos PostgreSQL y de archivos.
- Evaluar si conviene mover la imagen de `ficha_tecnica` a almacenamiento local (como en Compras/Pedidos) para no hacer crecer la base de datos con binarios.