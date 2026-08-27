# Matriz de pruebas del sistema

Usar esta matriz junto con `skills/qa-tester/SKILL.md`. Se deben ejecutar todas las pruebas que apliquen al cambio.

| Módulo | Pruebas mínimas |
| --- | --- |
| Compilación | `dotnet build`, `dotnet test`, `npm run build` y `npm run lint`. |
| Inicio de sesión | Credenciales correctas, contraseña incorrecta, usuario inactivo, token inválido/vencido y cierre de sesión. |
| Usuarios | Crear, editar, eliminar, correo, estado, grupos y restricción de visibilidad entre administradores cuando aplique. |
| Grupos | Crear, editar, eliminar, asignar usuarios, duplicados, permisos y usuarios sin grupo. |
| Compras | Obligatorios, fechas, receptor, catálogos, búsqueda, alta, edición, eliminación, auditoría y actualización de listas. |
| Comprobantes | Destinatario, copia al origen, contenido del comprobante, error de configuración y bloqueo tras envío. |
| Empresas | Nombre, RIF obligatorio, RIF duplicado con distinto nombre, Oriente/Occidente/Aliada y edición. |
| Catálogos | CRUD de cada catálogo, duplicados, elementos en desplegables y permisos de Oficina/Pedidos. |
| Pedidos | Producto nuevo/repetido, grupo nuevo/existente, todos los campos, filtro, paginación, edición y eliminación. |
| Pedidos enviados | Enviar, fecha/estado, bloqueo de editar/eliminar, lectura y filtro por estado. |
| Pedidos agrupados | Crear grupo, añadir varios productos, desplegar subpedidos y exportación individual/grupal. |
| Imágenes | JPEG/PNG/WebP, 15 MB, archivo inválido, reemplazo, eliminación, miniatura, autorización y rutas inexistentes. |
| Reportes | Excel/PDF individual y de grupo; orden de columnas, símbolos RMB/CNY, dos imágenes y datos completos. |
| Tiempo real | Dos sesiones: crear/editar/enviar y confirmar reflejo sin recarga manual. |
| API | 200/201/204 correctos; 400 de validación, 401 sin token, 403 sin permiso, 404 de recurso inexistente y 409 de conflicto. |
| Base de datos | Migración idempotente, respaldo previo, restricciones, claves foráneas, auditoría y preservación de registros. |
| Seguridad | Intentos no destructivos de inyección en entradas, XSS reflejado, validación de archivos, CORS y secretos fuera de Git. |
| Producción | API responde, OpenAPI responde, Caddy sin 502, variables de entorno presentes y frontend usa URL correcta. |

## Casos de borde obligatorios

- Campos requeridos vacíos, espacios, valores máximos y caracteres especiales.
- Fechas inválidas, llegada anterior a salida y zonas horarias.
- RIF y nombres repetidos con diferencias de mayúsculas/minúsculas.
- Usuario sin grupo, inactivo o sin correo.
- Producto sin imágenes, con una imagen y con ambas imágenes.
- Un recurso ya enviado que recibe petición directa de editar o eliminar.
- Consultas paginadas sin resultados, última página y filtros combinados.

