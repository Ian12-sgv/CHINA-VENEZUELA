# Guía de uso y operación

Esta guía explica cómo usar, ejecutar, actualizar y validar el Sistema China → Venezuela.

## 1. Acceso al sistema

1. Abrir la dirección del frontend publicada en Netlify.
2. Escribir el nombre de usuario y la contraseña entregados por el administrador.
3. Seleccionar **Iniciar sesión**.

No existe registro público. Los usuarios se crean desde la administración del sistema por los usuarios autorizados.

## 2. Permisos por grupo

| Grupo o usuario | Secciones disponibles |
| --- | --- |
| `oficina` | Recibos de compra y Campos operativos. |
| `Pedidos` | Pedidos, catálogo de productos, pedidos agrupados y Campos operativos. |
| `MS` / `SIS` | Todas las secciones, usuarios y grupos. |

Si una sección no aparece en el menú, el usuario no pertenece al grupo que la puede utilizar.

## 3. Crear un usuario

Solo los administradores pueden hacerlo.

1. Entrar en **Usuarios**.
2. Seleccionar crear usuario.
3. Indicar nombre, código de usuario, correo, contraseña y grupos.
4. Guardar.
5. Entregar las credenciales de forma segura al nuevo usuario.

El correo se utiliza para recibir comprobantes cuando el usuario sea origen o receptor de una compra.

## 4. Administrar grupos

1. Entrar en **Grupos de usuarios**.
2. Para crear un grupo, escribir el nombre y seleccionar **Agregar grupo**.
3. Para asignarlo, elegir un usuario, marcar los grupos correspondientes y seleccionar **Guardar asignación**.
4. Usar Editar o Eliminar cuando sea necesario.

No se deben eliminar grupos que tengan usuarios activos sin revisar antes su impacto en los permisos.

## 5. Campos operativos

La sección **Campos operativos** alimenta los desplegables de compras y pedidos.

Permite administrar:

- Contenedor compartido.
- Empresa.
- Marca de bulto.
- Aduana.
- Puerto de llegada.
- Agentes.

### Empresas

Al crear una empresa se debe registrar:

- Nombre de la empresa.
- RIF.
- Clasificación: Oriente, Occidente o Aliada.

El RIF no se puede repetir, aunque el nombre de la empresa sea diferente.

## 6. Registrar una compra recibida

1. Entrar en **Recibos de compra**.
2. Completar nombre y número del contenedor.
3. Seleccionar empresa, contenedor compartido, puerto, marca de bulto y aduana cuando aplique.
4. Registrar fecha de salida y, si existe, fecha de llegada.
5. Seleccionar el usuario receptor.
6. Agregar la descripción si es necesaria.
7. Seleccionar **Registrar recibo de compra**.

El recibo aparecerá en el historial y las demás sesiones conectadas se actualizarán automáticamente.

### Buscar compras

El buscador del historial revisa contenedor, empresa, salida, llegada, puerto, marca y receptor.

### Enviar comprobante

1. En el historial, seleccionar **Enviar** en la columna Comprobante.
2. El sistema envía el comprobante al correo del receptor y copia al correo del usuario que creó la compra.
3. Al enviarse correctamente, la compra queda bloqueada para editar o eliminar.

Si el envío no está configurado, revisar las variables `Resend__ApiKey` y `Resend__RemitenteCorreo` en el servidor.

## 7. Registrar un producto/pedido

1. Entrar en **Pedidos**.
2. Elegir si se agregará a un grupo existente o a un nuevo grupo.
3. Si es nuevo, el sistema asigna automáticamente el siguiente correlativo: `001`, `002`, etc.
4. Completar los campos del producto:
   - Tipo de producto: Nuevo o Repetido.
   - Fecha de registro del pedido (obligatoria).
   - Fecha de inicio de fabricación (opcional).
   - Agente.
   - Imagen de fábrica.
   - Fábrica.
   - Composición de tela.
   - Color para fabricar.
   - Marca del producto.
   - Curva de talla.
   - Pack por cajas.
   - Cantidad de unidades.
   - Una o varias marcas de bulto, con su cantidad independiente para cada marca.
   - Imagen del producto terminado.
   - Referencia asignada.
   - Código de barra asignado.
   - Precio RMB, total RMB y cantidad DOZ.
5. Seleccionar **Registrar producto**.

Las imágenes permitidas son JPEG, PNG y WebP, con máximo de 15 MB cada una. La API reserva margen técnico para el formulario multipart, pero no acepta archivos superiores a 15 MB.

## 8. Catálogo y pedidos agrupados

En **Catálogo de productos** se puede:

- Filtrar por grupo de pedido, estado o texto.
- Abrir un pedido agrupado y revisar sus subpedidos.
- Editar o eliminar productos pendientes.
- Duplicar un subpedido dentro del mismo grupo: se solicita un código de barra nuevo y único; el nuevo registro queda pendiente y copia las imágenes de fábrica y del producto terminado en archivos independientes.
- Actualizar o eliminar las imágenes.
- Marcar un producto como enviado.
- Descargar Excel o PDF del grupo completo o de un subpedido.
- Mostrar cada marca de bulto usada como encabezado de columna y su cantidad debajo, tanto en catálogo como en las exportaciones.

Los documentos de exportación incluyen las imágenes y los valores monetarios en RMB/CNY (`¥`).

Cuando un pedido se envía, sus acciones de editar y eliminar quedan bloqueadas.

## 9. Ejecutar localmente

### Requisitos

- .NET SDK 10 y ASP.NET Core Runtime 10.
- PostgreSQL disponible.
- Node.js y npm.
- Base de datos `china_venezuela` actualizada.

### Backend

En PowerShell, desde la raíz del proyecto:

```powershell
$env:ConnectionStrings__PostgreSql = 'Host=localhost;Port=5432;Database=china_venezuela;Username=postgres;Password=TU_CLAVE'
$env:Jwt__Key = 'CLAVE_LARGA_Y_SEGURA'
$env:ASPNETCORE_URLS = 'http://127.0.0.1:5081'
dotnet run --project src\ChinaVenezuela.Api
```

### Frontend

En otra consola:

```powershell
cd frontend
npm install
npm run dev
```

Abrir `http://127.0.0.1:5173`.

## 10. Validar el sistema

Antes de publicar cambios, ejecutar:

```powershell
dotnet build ChinaVenezuela.slnx --no-restore
dotnet test ChinaVenezuela.slnx --no-build

cd frontend
npm run build
```

Comprobar además:

1. Inicio de sesión válido e inválido.
2. Permisos de Oficina, Pedidos y administrador.
3. Registro y búsqueda de compras.
4. Envío de comprobante y bloqueo posterior.
5. Carga de ambas imágenes de producto.
6. Crear/editar/enviar producto y bloqueo posterior.
7. Descarga de Excel/PDF.
8. Actualización automática de otra sesión conectada.

## 11. Actualizar la base de datos en el servidor

> Siempre crear respaldo antes de ejecutar una migración.

### Crear respaldo

```powershell
$pgBin = 'C:\Program Files\PostgreSQL\18\bin'
$backupDir = 'C:\Users\Administrator\Documents\servicios IAN\respaldos-china'
$fecha = Get-Date -Format 'yyyyMMdd-HHmmss'

New-Item -ItemType Directory -Force -Path $backupDir
& "$pgBin\pg_dump.exe" -h localhost -p 5432 -U postgres -W `
  -F c -f "$backupDir\china_venezuela_$fecha.dump" china_venezuela
```

### Aplicar actualización idempotente

Copiar `actualizar-base-servidor-actual.sql` al servidor y ejecutar:

```powershell
$pgBin = 'C:\Program Files\PostgreSQL\18\bin'
$scriptSql = 'C:\Users\Administrator\Documents\servicios IAN\actualizar-base-servidor-actual.sql'

& "$pgBin\psql.exe" -h localhost -p 5432 -U postgres -d china_venezuela -W `
  -v ON_ERROR_STOP=1 -f $scriptSql
```

No usar `psql -f` con un archivo `.dump`; los respaldos en formato custom se inspeccionan/restauran con `pg_restore`.

## 12. Publicar backend

1. Generar o tomar la carpeta `backend-publicar-pedidos-2`.
2. Copiar su contenido al servidor, sin incluir credenciales dentro de los archivos.
3. Configurar las variables de entorno de máquina:

```powershell
[Environment]::SetEnvironmentVariable('ConnectionStrings__PostgreSql', 'CADENA_DE_CONEXION', 'Machine')
[Environment]::SetEnvironmentVariable('Jwt__Key', 'CLAVE_SEGURA', 'Machine')
[Environment]::SetEnvironmentVariable('Resend__ApiKey', 'CLAVE_RESEND', 'Machine')
[Environment]::SetEnvironmentVariable('Resend__RemitenteCorreo', 'comprobantes@DOMINIO', 'Machine')
[Environment]::SetEnvironmentVariable('Resend__RemitenteNombre', 'Recibos de compra', 'Machine')
```

4. Reiniciar el proceso o servicio que ejecuta `ChinaVenezuela.Api.exe`.
5. Confirmar que el puerto 5081 responde.

```powershell
Test-NetConnection 127.0.0.1 -Port 5081
Invoke-WebRequest 'https://api-china.apipalacio.com/openapi/v1.json' -UseBasicParsing
```

## 13. Configurar Caddy

El bloque principal es:

```caddyfile
api-china.apipalacio.com {
    encode zstd gzip
    reverse_proxy 127.0.0.1:5081
}
```

Después de editar el Caddyfile:

```powershell
caddy validate --config C:\caddy\Caddyfile --adapter caddyfile
caddy reload --config C:\caddy\Caddyfile --adapter caddyfile
```

## 14. Publicar frontend en Netlify

Configuración esperada:

| Campo | Valor |
| --- | --- |
| Repositorio | `Ian12-sgv/CHINA-VENEZUELA` |
| Rama | `main` |
| Base directory | `frontend` |
| Build command | `npm run build` |
| Publish directory | `dist` |
| Variable | `VITE_API_URL=https://api-china.apipalacio.com/api` |

Un `push` a `main` debería iniciar un despliegue automático. Si no ocurre, revisar en Netlify la conexión con GitHub y el registro del último deploy.

## 15. Seguridad y buenas prácticas

- No incluir claves, contraseñas, `.env`, logs, imágenes o publicaciones backend en Git.
- No editar ni eliminar registros ya enviados.
- Validar cada actualización de base de datos primero con respaldo.
- Usar Pull Requests para revisar cambios antes de fusionarlos a `main` cuando la rama esté protegida.
- Conservar los archivos de imagen fuera de PostgreSQL y mantener respaldo del directorio de imágenes.
- Revisar periódicamente logs de Caddy, API y uso de disco.

## 16. Solución rápida de problemas

| Problema | Verificación inicial |
| --- | --- |
| Netlify muestra cambios antiguos | Confirmar que el commit llegó al repositorio y rama configurados para deploy. |
| Error CORS / `Failed to fetch` | Revisar API activa, dominio permitido en CORS y `VITE_API_URL`. |
| Error 502 de API | Confirmar `Test-NetConnection 127.0.0.1 -Port 5081` y proceso backend iniciado. |
| Error 500 al iniciar sesión | Revisar cadena PostgreSQL, `Jwt__Key`, logs de API y estado de la base de datos. |
| No aparecen grupos | Confirmar API, tablas `grupo`/`grupo_usuario` y permisos. |
| Correo no se envía | Revisar variables `Resend__ApiKey` y `Resend__RemitenteCorreo`; reiniciar el proceso API. |
| Imagen no carga | Revisar límite de 15 MB, extensión permitida, almacenamiento local y autorización. |
| No se actualiza una lista | Revisar conexión SignalR, token vigente y el refresco de TanStack Query. |

Para información de arquitectura, modelo de datos y decisiones técnicas, consultar [contexto.md](contexto.md).
## 17. Mantenimiento obligatorio de esta guía

Cada vez que se agregue, modifique o elimine una funcionalidad del sistema, se debe actualizar esta guía dentro del mismo cambio de código.

La actualización debe incluir, según corresponda:

- Cómo usa la nueva función el usuario.
- Qué permisos o grupos pueden acceder a ella.
- Campos, validaciones y reglas de negocio relevantes.
- Nuevos endpoints, variables de entorno o requisitos de configuración.
- Cambios de base de datos, migraciones o pasos de despliegue.
- Pruebas manuales mínimas y posibles errores conocidos.

Un cambio funcional no se considera terminado hasta que `guia.md` y, si cambia la arquitectura o el modelo de datos, `contexto.md` estén actualizados.
## 18. Agente QA y Testing

Antes de entregar una función, corrección, migración o Pull Request se debe invocar el agente `qa-tester`.

El agente ejecuta compilación, pruebas, revisión funcional, permisos, API, base de datos, imágenes, reportes, tiempo real y verificaciones de seguridad básicas según el cambio. Si detecta una falla reproducible, debe crear un archivo individual dentro de `qa/errores/`:

```text
error-qa-AAAA-MM-DD-HHMM-descripcion-corta.md
error-testing-AAAA-MM-DD-HHMM-descripcion-corta.md
```

No se debe aprobar un cambio crítico o de alto riesgo mientras exista un reporte abierto sin una aceptación explícita del riesgo. La definición completa del agente está en `skills/qa-tester/SKILL.md`.
## 19. Agente de arreglo

Cuando `qa-tester` genere un reporte abierto en `qa/errores/`, se debe invocar el agente `arreglo` indicando la ruta exacta del archivo.

El agente `arreglo` primero lee `contexto.md`, `guia.md`, el reporte y la matriz QA; después reproduce el problema, aplica la corrección mínima, ejecuta las pruebas pertinentes y actualiza el mismo reporte con evidencia. Un error solo se cierra cuando la prueba que fallaba pasa de nuevo.

Ejemplo:

```text
Usa el agente arreglo para corregir qa/errores/error-testing-AAAA-MM-DD-HHMM-descripcion.md.
```

La definición completa está en `skills/arreglo/SKILL.md`.
