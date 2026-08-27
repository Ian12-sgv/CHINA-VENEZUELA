# [MEDIO] La suite automatizada no cubre los flujos críticos del sistema

- ID: TEST-2026-08-27-004
- Tipo: Testing / Cobertura
- Estado: Abierto
- Fecha: 2026-08-27 09:13 (UTC-4)
- Módulo: Calidad transversal
- Entorno: Local
- Reportado por: qa-tester

## Descripción

La ejecución de pruebas pasó, pero la solución contiene solamente tres pruebas unitarias. No existen pruebas automatizadas de integración para API, autorización, PostgreSQL, SignalR, correo, cargas de imágenes, exportaciones PDF/Excel ni flujos de bloqueo tras envío. Tampoco hay pruebas automatizadas de interfaz.

## Pasos para reproducir

1. Ejecutar `dotnet test ChinaVenezuela.slnx --no-build`.
2. Verificar que el resultado indica `Passed: 3, Total: 3`.
3. Revisar `tests/ChinaVenezuela.Application.Tests`.
4. Confirmar que no hay proyectos de integración/API ni pruebas frontend para los módulos listados.

## Resultado esperado

Los flujos críticos deben contar con pruebas automatizadas proporcionales al riesgo, como mínimo para autenticación/permisos, compras enviadas, pedidos enviados, reglas de RIF, cargas de imágenes, paginación y grupos de pedidos.

## Resultado actual

Solo se cubren tres pruebas unitarias de aplicación. La validación integral requiere operaciones manuales y un entorno configurado, por lo que no puede considerarse una regresión completamente verificada.

## Evidencia

- Comando ejecutado: `dotnet test ChinaVenezuela.slnx --no-build`.
- Resultado: 3 pruebas aprobadas de 3.
- No se hallaron proyectos de pruebas de integración o pruebas frontend en el repositorio.

## Severidad y riesgo

**Medio.** Cambios futuros pueden romper permisos, migraciones, correos, imágenes o exportaciones sin detección automática antes del despliegue.

## Corrección propuesta

1. Crear pruebas de integración de API con base de datos aislada.
2. Añadir pruebas de permisos para `oficina`, `Pedidos`, `MS` y `SIS`.
3. Probar bloqueos de editar/eliminar después de envío.
4. Añadir pruebas de archivos permitidos, 15 MB, rutas de imagen y 401.
5. Añadir pruebas frontend para filtros, actualizaciones en tiempo real y exportaciones.

## Verificación de corrección

- Fecha: Pendiente.
- Prueba repetida: Pendiente.
- Resultado: Pendiente.
- Responsable: Pendiente.

