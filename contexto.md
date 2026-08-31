# Contexto técnico — Sistema China → Venezuela

> Documento base para desarrollo, operación y transferencia del conocimiento.
> Última actualización: 27 de agosto de 2026.
> **Seguridad:** este archivo nunca debe contener contraseñas, cadenas de conexión completas, tokens JWT ni claves de Resend.

## 1. Ficha técnica

| Elemento | Descripción |
| --- | --- |
| Nombre | Sistema de Compras, Pedidos y Envíos China → Venezuela |
| Propósito | Centralizar la recepción de mercancía, la gestión de pedidos/productos, catálogos operativos, usuarios y comprobantes. |
| Arquitectura | Monolito modular con Clean Architecture ligera. |
| Frontend | React 19, TypeScript, Vite 8, TanStack Query, SignalR y Font Awesome. |
| Backend | ASP.NET Core 10 LTS, C#, Entity Framework Core 10, OpenAPI. |
| Base de datos | PostgreSQL, esquema `public`, convención `snake_case`. |
| Autenticación | JWT y contraseñas con hash PBKDF2. |
| Tiempo real | SignalR mediante `/hub/actualizaciones`. |
| Correo | Resend mediante API HTTP; comprobantes de compras. |
| Imágenes | Almacenamiento local fuera de PostgreSQL; metadatos en base de datos. |
| Frontend productivo | Netlify: `https://tracking-china.netlify.app` |
| API productiva | `https://api-china.apipalacio.com` |
| Proxy / TLS | Caddy redirige la API pública hacia el backend local. |

## 2. Objetivo y alcance

El sistema cubre el flujo operativo de mercancía enviada desde China hacia Venezuela y la preparación de pedidos de productos.

### Módulos incluidos

1. Inicio de sesión y administración de usuarios.
2. Grupos, asignación de permisos y control de acceso.
3. Compras recibidas: registro de contenedor, empresa, fechas, puerto, aduana, marca de bulto, receptor y comprobante.
4. Catálogos operativos: contenedor compartido, empresas, marcas de bulto, aduanas, puertos de llegada y agentes.
5. Pedidos / catálogo de productos: productos, grupos de pedidos, precios RMB, cantidades, imágenes y exportaciones.
6. Envío por correo de comprobantes de compras.
7. Auditoría de acciones relevantes.

### Reglas de negocio relevantes

- Un comprobante de compra enviado no puede editarse ni eliminarse.
- Un producto/pedido enviado no puede editarse ni eliminarse.
- Un subpedido agrupado puede duplicarse únicamente con un código de barra nuevo y único; conserva sus datos, grupo y ambas imágenes en archivos independientes.
- Los grupos nuevos de pedidos se nombran automáticamente con el correlativo 001, 002, etc.
- El RIF de una empresa debe ser único: no se permiten dos empresas con el mismo RIF.
- Las empresas pueden clasificarse como Oriente, Occidente o Aliadas.
- Las imágenes de producto aceptan JPEG, PNG y WebP, con máximo de 15 MB.
- Todas las fechas con hora se manejan y almacenan en UTC.
- Las entidades de base de datos no se exponen directamente desde la API; se usan contratos/DTOs.

## 3. Arquitectura

```text
Usuario
  │
  ├─ Navegador ──► Netlify / React + Vite
  │                    │ HTTPS + JWT
  │                    ├─ API REST
  │                    └─ SignalR
  │
  └────────────────► Caddy (HTTPS / reverse proxy)
                         │
                         ▼
                   ASP.NET Core 10 API
                    │       │       │
                    │       │       └─ Resend (comprobantes por correo)
                    │       └──────── Almacenamiento local de imágenes
                    ▼
                 PostgreSQL
```

### Capas backend

```text
API                 Controllers, JWT, OpenAPI, CORS, SignalR, correo
Application         Casos de uso, DTOs, validaciones e interfaces
Domain              Entidades y reglas de negocio
Infrastructure      EF Core, Npgsql, repositorios, migraciones e imágenes
```

## 4. Árbol de carpetas

```text
China/
├── frontend/                              # Aplicación React/Vite
│   ├── src/
│   │   ├── pages/                         # Vistas: compras, pedidos, usuarios, grupos, catálogos
│   │   ├── components/                    # Componentes reutilizables
│   │   ├── hooks/                         # SignalR y actualizaciones en tiempo real
│   │   ├── api.ts                         # Cliente HTTP de la API
│   │   ├── types.ts                       # Tipos TypeScript
│   │   ├── App.tsx                        # Navegación y permisos visuales
│   │   └── App.css / index.css            # Estilos
│   ├── public/                            # Iconos y recursos públicos
│   ├── package.json
│   └── vite.config.ts                     # Proxy local /api y /hub → :5081
├── src/
│   ├── ChinaVenezuela.Api/                # Entrada HTTP del backend
│   │   ├── Controllers/
│   │   ├── Auth/
│   │   ├── Comprobantes/
│   │   ├── Hubs/
│   │   ├── ExceptionHandling/
│   │   ├── Program.cs
│   │   └── appsettings*.json
│   ├── ChinaVenezuela.Application/        # Servicios, interfaces, contratos y validaciones
│   ├── ChinaVenezuela.Domain/             # Entidades de negocio
│   └── ChinaVenezuela.Infrastructure/     # DbContext, EF, repositorios y migraciones
├── tests/
│   └── ChinaVenezuela.Application.Tests/  # Pruebas de aplicación
├── contexto/                              # Documentación funcional y técnica original
├── memoria/                               # Estado, decisiones, glosario y próximos pasos
├── reglas/                                # Alcance y forma de trabajo
├── datos/imagenes/                        # Archivos locales de imágenes (ignorado por Git)
├── deploy/                                # Scripts auxiliares de actualización
├── backend-publicar-pedidos-2/            # Publicación Release para llevar al servidor (ignorada por Git)
├── actualizar-base-servidor-actual.sql    # Migración idempotente para producción
├── ChinaVenezuela.slnx                    # Solución .NET
├── README.md
└── contexto.mb                            # Este documento
```

## 5. Modelo de datos

### Tablas principales

| Tabla PostgreSQL | Responsabilidad |
| --- | --- |
| `usuario` | Credenciales, nombre, correo, estado y metadatos de usuarios. |
| `grupo` | Catálogo de grupos de seguridad. |
| `grupo_usuario` | Relación entre usuario y grupo. |
| `compra_recibida` | Recepción de mercancía, empresa, contenedor, fechas, receptor y estado de comprobante. |
| `empresa` | Empresas importadoras, RIF único y clasificación geográfica/aliada. |
| `contenedor_compartido` | Opciones de contenedor compartido. |
| `marca_bulto` | Marcas de bulto. |
| `aduana` | Aduanas disponibles. |
| `puerto_llegada` | Puertos de llegada disponibles. |
| `agente_pedido` | Agentes disponibles para pedidos. |
| `producto_pedido` | Detalle de cada producto/pedido. |
| `producto_pedido_bulto` | Marcas de bulto y cantidades asociadas a cada producto/pedido. |
| `producto_pedido_imagen` | Metadatos de imágenes de fábrica y producto terminado. |
| `pedidos` | Cabecera de un pedido agrupado. |
| `pedidos_grupos` | Relación entre pedido agrupado y productos. |
| `registro_auditoria` | Registro de creación, actualización y eliminación. |
| `registro_precio_pedido` | Estructura histórica de precios; no es una pantalla operativa actual. |
| `__EFMigrationsHistory` | Historial técnico de migraciones Entity Framework. |

### Relaciones principales

```text
usuario ──< grupo_usuario >── grupo
usuario ──< compra_recibida (receptor)
empresa ──< compra_recibida
contenedor_compartido ──< compra_recibida
marca_bulto ──< compra_recibida

pedidos ──< pedidos_grupos >── producto_pedido ──< producto_pedido_imagen
                                      └──< producto_pedido_bulto >── marca_bulto
usuario ──< producto_pedido (creado_por_codigo_usuario)
```

### Campos funcionales de `producto_pedido`

- Tipo de producto: Nuevo o Repetido.
- Fecha de registro del pedido y fecha opcional de inicio de fabricación.
- Agente.
- Imagen de fábrica.
- Fábrica.
- Composición de tela.
- Color para fabricar.
- Marca del producto.
- Curva de talla.
- Pack por cajas.
- Cantidad de unidades.
- Una o varias marcas de bulto, cada una con su cantidad.
- Imagen del producto terminado.
- Referencia asignada.
- Código de barra asignado.
- Precio RMB, total RMB y cantidad DOZ.
- Estado de envío y fecha de envío UTC.

## 6. Roles y permisos

| Perfil / grupo | Acceso |
| --- | --- |
| `MS` y `SIS` | Administración de usuarios y grupos; acceso a compras, pedidos y catálogos. |
| `oficina` | Compras recibidas y catálogos operativos. |
| `Pedidos` | Pedidos, catálogo de productos y catálogos operativos. |

Las políticas del backend son `GestionGrupos`, `AccesoCompras`, `AccesoPedidos` y `AccesoOperativo`. Los controles visuales del frontend complementan estas reglas, pero la autorización real siempre se valida en la API.

## 7. API REST

La especificación OpenAPI está disponible en:

```text
https://api-china.apipalacio.com/openapi/v1.json
```

| Área | Rutas principales |
| --- | --- |
| Autenticación | `POST /api/auth/iniciar-sesion` |
| Cuenta | `GET /api/cuenta` |
| Compras | `GET/POST /api/compras-recibidas`, `GET/PUT/DELETE /api/compras-recibidas/{id}` |
| Comprobantes | `POST /api/compras-recibidas/{id}/comprobante/enviar` |
| Catálogos | CRUD en `/api/empresas`, `/api/aduanas`, `/api/puertos-llegada`, `/api/marcas-bulto`, `/api/contenedores-compartidos` |
| Agentes | CRUD en `/api/agentes` |
| Usuarios | CRUD y grupos en `/api/usuarios` |
| Grupos | CRUD en `/api/grupos` |
| Receptores | `GET /api/receptores` |
| Pedidos | Productos, grupos, envío e imágenes bajo `/api/pedidos/...` |
| Tiempo real | Hub autenticado `/hub/actualizaciones` |

## 8. Variables de entorno y configuración

Las variables deben configurarse en el equipo/servidor, nunca en Git:

```text
ConnectionStrings__PostgreSql      # Cadena de PostgreSQL
Jwt__Key                          # Clave aleatoria larga para firmar JWT
Resend__ApiKey                    # API key de Resend
Resend__RemitenteCorreo           # Dirección de correo remitente
Resend__RemitenteNombre           # Nombre visible del remitente
ASPNETCORE_URLS                   # Ejemplo: http://127.0.0.1:5081
```

En Netlify se utiliza:

```text
VITE_API_URL=https://api-china.apipalacio.com/api
```

Las imágenes se almacenan de forma local por medio de `ImagenesOptions`; la ruta debe ser una carpeta persistente, con permisos de lectura/escritura para el proceso de la API y sin acceso público directo fuera de los endpoints autenticados.

## 9. Ejecución local

### Requisitos

- .NET SDK 10 y runtime ASP.NET Core 10.
- PostgreSQL accesible.
- Node.js y npm.
- Base de datos creada y actualizada.

### Backend

```powershell
$env:ConnectionStrings__PostgreSql = 'Host=localhost;Port=5432;Database=china_venezuela;Username=postgres;Password=TU_CLAVE'
$env:Jwt__Key = 'CLAVE_SEGURA_DE_AL_MENOS_32_CARACTERES'
$env:ASPNETCORE_URLS = 'http://127.0.0.1:5081'

dotnet run --project src\ChinaVenezuela.Api
```

### Frontend

```powershell
cd frontend
npm install
npm run dev
```

Direcciones locales esperadas:

```text
Frontend: http://127.0.0.1:5173
API:      http://127.0.0.1:5081
OpenAPI:  http://127.0.0.1:5081/openapi/v1.json
```

## 10. Base de datos, migraciones y respaldo

### Flujo seguro para producción

1. Detener o poner en mantenimiento la API si la actualización lo requiere.
2. Crear un respaldo PostgreSQL con `pg_dump` en `C:\Users\Administrator\Documents\servicios IAN\respaldos-china`.
3. Validar que el archivo de respaldo exista y tenga contenido.
4. Aplicar `actualizar-base-servidor-actual.sql` usando `psql` y `ON_ERROR_STOP=1`.
5. Verificar tablas y `__EFMigrationsHistory`.
6. Publicar/reiniciar la API con la carpeta Release actual.
7. Probar inicio de sesión, compras, pedidos e imágenes.

El script de actualización es idempotente: crea o altera solo lo faltante según el historial de migraciones. Aun así, el respaldo es obligatorio antes de aplicarlo.

### Comandos de referencia

```powershell
# Compilar
dotnet build ChinaVenezuela.slnx --no-restore

# Generar publicación para el servidor
dotnet publish src\ChinaVenezuela.Api\ChinaVenezuela.Api.csproj -c Release -o backend-publicar-pedidos-2

# Generar script idempotente (requiere dotnet-ef instalado)
dotnet ef migrations script --idempotent `
  --project src\ChinaVenezuela.Infrastructure `
  --startup-project src\ChinaVenezuela.Api `
  --output actualizar-base-servidor-actual.sql
```

## 11. Despliegue

### Frontend

- Repositorio conectado a Netlify: `Ian12-sgv/CHINA-VENEZUELA`.
- Rama de producción: `main`.
- Directorio base: `frontend`.
- Comando de compilación: `npm run build`.
- Directorio publicado: `dist` (relativo al directorio base).
- Variable `VITE_API_URL` configurada con la URL pública de la API.

### Backend

- Copiar el contenido de `backend-publicar-pedidos-2` al servidor en la carpeta de servicio elegida.
- Instalar .NET / ASP.NET Core Runtime 10 sin eliminar los runtimes anteriores.
- Definir las variables de entorno del proceso o de máquina.
- Ejecutar `ChinaVenezuela.Api.exe` escuchando en `127.0.0.1:5081`.
- Caddy debe realizar `reverse_proxy 127.0.0.1:5081` bajo `api-china.apipalacio.com`.

Después de cambiar el Caddyfile:

```powershell
caddy validate --config C:\caddy\Caddyfile --adapter caddyfile
caddy reload --config C:\caddy\Caddyfile --adapter caddyfile
```

## 12. Estrategia Git

| Repositorio | Uso |
| --- | --- |
| `Ian12-sgv/CHINA-VENEZUELA` | Repositorio conectado a Netlify; `main` publica el frontend. |
| `PDB-sis/tracking` | Repositorio colaborativo y flujo con ramas protegidas. |

Principios:

- No subir secretos, publicaciones `backend-publicar*`, imágenes locales, logs ni archivos RAR.
- Desarrollar en una rama de trabajo.
- Validar antes de fusionar hacia `main`.
- Cuando exista una regla de protección, usar Pull Request y aprobación.
- Mantener ambos repositorios alineados solo cuando se autorice explícitamente.

## 13. Convenciones de desarrollo

- PascalCase en C#; `snake_case` para tablas/columnas PostgreSQL.
- Usar `Guid` como identificadores principales cuando corresponda.
- Usar DTOs/contratos de entrada y salida; no devolver entidades EF directamente.
- Manejar errores HTTP mediante `ProblemDetails` y el manejador global de excepciones.
- Validar entradas antes de persistir datos.
- Fechas con hora: `DateTimeOffset` UTC. Fechas de negocio sin hora: `DateOnly`.
- Usar TanStack Query para caché y actualización de la interfaz.
- Publicar actualizaciones mediante SignalR cuando una operación de otro usuario altere la vista.
- Mantener control de permisos en backend; esconder botones no reemplaza una política API.

## 14. Seguridad

- Contraseñas almacenadas con hash PBKDF2; nunca guardar texto plano.
- JWT firmado con `Jwt__Key` configurada fuera del repositorio.
- CORS limitado al dominio Netlify permitido; si cambia el dominio, actualizar la política de CORS en `Program.cs`.
- Caddy entrega HTTPS, HSTS y encabezados de seguridad.
- Las cargas de archivos validan tipo, tamaño y se almacenan fuera de la base de datos.
- Los endpoints de imágenes requieren autorización de pedidos.
- Las acciones de cambios se registran en auditoría.
- Respaldar PostgreSQL antes de toda migración productiva.

## 15. Pruebas y control de calidad

```powershell
# Backend
dotnet build ChinaVenezuela.slnx --no-restore
dotnet test ChinaVenezuela.slnx --no-build

# Frontend
cd frontend
npm run build
```

Validación manual mínima:

1. Iniciar sesión con un usuario válido e invalidar acceso con uno sin permisos.
2. Crear, editar y eliminar un dato de catálogo autorizado.
3. Registrar una compra y comprobar que aparece inmediatamente en el historial.
4. Enviar un comprobante y verificar bloqueo de edición/eliminación.
5. Crear/editar producto, cargar ambas imágenes y probar límites/tipos de archivo.
6. Enviar un pedido y comprobar bloqueo de modificación.
7. Exportar Excel/PDF para un producto y para un pedido agrupado.
8. Confirmar que otras sesiones reciben actualización por SignalR.

## 16. Estado actual y próximos pasos

### Implementado

- Compras recibidas, catálogos operativos, empresas con RIF y clasificación.
- Usuarios, grupos y permisos de compras/pedidos.
- Pedidos agrupados y productos con información logística/comercial.
- Dos imágenes por producto, alojadas localmente.
- Precio/total RMB y cantidad DOZ.
- Exportación de pedidos a PDF y Excel.
- Comprobantes por correo mediante Resend.
- Actualización en tiempo real y auditoría.
- Publicación backend preparada en `backend-publicar-pedidos-2`.

### Pendientes recomendados

- Automatizar el inicio del backend como servicio de Windows o tarea programada supervisada.
- Añadir pruebas de integración para permisos, carga de imágenes y migraciones.
- Definir monitoreo de logs, uso de disco y respaldo programado.
- Elaborar política de retención/archivo para imágenes cuando el volumen crezca.
- Implementar controles de recuperación de cuenta y rotación periódica de secretos.
- Evaluar almacenamiento de imágenes dedicado si el volumen supera la capacidad local administrable.

## 17. Decisiones técnicas

- **ASP.NET Core 10 + EF Core + Npgsql:** tipado fuerte, migraciones y compatibilidad natural con PostgreSQL.
- **React + Vite:** interfaz rápida y desacoplada de la API.
- **PostgreSQL:** persistencia relacional, restricciones e integridad de datos.
- **Caddy:** proxy inverso y certificados HTTPS automáticos.
- **Netlify:** entrega sencilla del frontend estático.
- **Resend:** servicio de correo por API, sin exponer credenciales en el navegador.
- **Imágenes fuera de la BD:** reduce tamaño y presión sobre PostgreSQL; la base guarda metadatos/ruta.
- **SignalR:** evita tener que recargar manualmente las listas después de cambios.


### Duplicacion de subpedidos e imagenes

Al duplicar un subpedido, el sistema conserva sus datos y grupo. Tambien copia la imagen de fabrica y la imagen del producto terminado a archivos locales nuevos. Por ello, actualizar o eliminar una imagen del duplicado no modifica el subpedido original.