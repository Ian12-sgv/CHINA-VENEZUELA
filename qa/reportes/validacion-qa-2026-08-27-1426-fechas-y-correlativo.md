# Validación QA — fechas de pedido y correlativo de grupos

- Fecha: 2026-08-27 14:26 (UTC-4)
- Entorno: Local
- Alcance: fechas de registro/inicio de fabricación, migración PostgreSQL y correlativo de grupos.
- Decisión: **Aprobado con riesgos**.

## Pruebas ejecutadas

| Verificación | Resultado | Evidencia |
| --- | --- | --- |
| Compilación .NET | Correcta | `dotnet build ChinaVenezuela.slnx --no-restore`, 0 advertencias/errores. |
| Pruebas .NET | Correctas | `dotnet test ChinaVenezuela.slnx --no-build`, 7 aprobadas. |
| Build frontend | Correcto | `npm.cmd run build` desde `frontend`. |
| Lint frontend | Correcto con advertencias previas | `npm.cmd run lint`: 2 advertencias de dependencias de hooks, sin errores. |
| OpenAPI local | Correcto | `GET /openapi/v1.json` devolvió 200 e incluye `fechaRegistroPedido` y `fechaInicioFabricacion`. |
| Autorización API | Correcta | `GET /api/pedidos/productos` sin token devolvió 401. |
| Migración local | Correcta | `20260827152421_AgregarFechasRegistroEInicioFabricacionPedido` aparece aplicada. |

## Pruebas no ejecutadas

- Alta, edición y consulta de un producto mediante API autenticada: no se usaron credenciales de prueba ni se alteraron datos locales existentes durante QA.
- Validación visual del formulario y actualización en tiempo real entre dos sesiones: requiere una sesión de prueba separada.
- Exportación e imágenes: no se modificaron registros existentes fuera del alcance inmediato.

## Hallazgos

- Abierto: [QA-2026-08-27-001](../errores/error-qa-2026-08-27-1426-correlativo-contexto.md), severidad baja, inconsistencia entre `contexto.md` y el formato correlativo actual.

## Riesgos

- El correlativo se calcula a partir de los nombres de grupos existentes. Dos creaciones simultáneas podrían requerir una protección adicional a nivel de base de datos si el sistema tendrá alta concurrencia.
- Las pruebas funcionales autenticadas quedan pendientes hasta contar con datos/credenciales de prueba autorizados.
