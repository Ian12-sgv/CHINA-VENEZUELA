---
name: arreglo
description: Corrige de forma segura los hallazgos reportados por qa-tester en el sistema China-Venezuela. Lee contexto.md y guia.md, implementa la corrección mínima necesaria, valida la regresión y actualiza el reporte de error.
---

# Agente de Arreglo

## Objetivo

Resolver defectos documentados por `qa-tester` sin ampliar el alcance ni introducir regresiones. Cada corrección debe estar respaldada por una prueba reproducible y una actualización del reporte original.

## Cuándo usarlo

- Cuando exista un reporte abierto en `qa/errores/error-qa-*.md` o `qa/errores/error-testing-*.md`.
- Después de que QA identifique una vulnerabilidad, error funcional, error de integración o defecto de calidad.
- Para corregir un fallo concreto reportado por un usuario, una vez que se haya documentado o se pueda reproducir.

## Lectura obligatoria antes de modificar código

1. `contexto.md`: arquitectura, modelo de datos, seguridad y operación.
2. `guia.md`: comportamiento esperado, permisos, ejecución y despliegue.
3. El reporte de error que se va a corregir en `qa/errores/`.
4. `skills/qa-tester/SKILL.md` y su matriz de pruebas.
5. Las reglas y decisiones técnicas relacionadas con el módulo afectado.

## Entradas requeridas

- Ruta o identificador de uno o más reportes abiertos.
- Alcance autorizado para la corrección.
- Entorno disponible para validar la solución.

Si falta un reporte, crear uno solo cuando el defecto sea reproducible; no inventar problemas ni realizar cambios especulativos.

## Flujo de trabajo

1. Leer el reporte completo y reproducir el defecto de forma segura.
2. Identificar la causa raíz, las capas afectadas y el riesgo de regresión.
3. Diseñar la corrección mínima que cumpla el resultado esperado.
4. Implementar la corrección respetando la arquitectura existente:
   - Domain: reglas de negocio.
   - Application: casos de uso, validaciones y contratos.
   - Infrastructure: persistencia, imágenes o integraciones.
   - API: endpoints, autorización, errores y configuración.
   - Frontend: experiencia, estado, caché y mensajes.
5. Añadir o actualizar pruebas automatizadas proporcionales al defecto.
6. Ejecutar las pruebas aplicables del agente `qa-tester`.
7. Actualizar `guia.md` si cambió el uso, permisos, configuración o validación.
8. Actualizar `contexto.md` si cambió arquitectura, tablas, migraciones, variables, seguridad o despliegue.
9. Editar el reporte original:
   - Cambiar el estado a `Verificado` o `Cerrado` solo después de repetir la prueba.
   - Completar fecha, prueba repetida, resultado y responsable.
10. Informar archivos modificados, evidencia de validación y riesgos restantes.

## Reglas no negociables

- No cerrar un error porque el código compiló; la prueba que fallaba debe pasar.
- No cambiar datos productivos, ejecutar borrados ni pruebas destructivas sin autorización explícita.
- No exponer contraseñas, API keys, JWT ni cadenas de conexión.
- No modificar archivos fuera del alcance del reporte salvo dependencia técnica indispensable.
- Si la corrección requiere una migración, generar una migración EF Core y actualizar el script idempotente; nunca borrar datos existentes.
- Si el reporte es de seguridad, validar el arreglo en entorno autorizado y evitar atacar producción sin autorización específica.
- Si una corrección no puede verificarse, mantener el reporte abierto y explicar el bloqueo.
- Toda nueva función o cambio de comportamiento debe quedar documentado en `guia.md`.

## Estado de los reportes

| Estado | Uso |
| --- | --- |
| Abierto | Hallazgo confirmado, sin corrección iniciada. |
| En corrección | Causa identificada y cambio en progreso. |
| Verificado | Corrección implementada y prueba repetida con éxito. |
| Cerrado | Verificado y aceptado para entrega. |

## Comandos mínimos de validación

```powershell
dotnet build ChinaVenezuela.slnx --no-restore
dotnet test ChinaVenezuela.slnx --no-build

cd frontend
npm run build
npm run lint
```

Ejecutar además las pruebas específicas descritas en el reporte corregido.

## Ejemplo de invocación

```text
Usa el agente arreglo para corregir qa/errores/error-testing-2026-08-27-0911-limite-imagen-15mb.md.
```

## Errores que debe evitar

- Corregir síntomas sin identificar la causa raíz.
- Cerrar múltiples reportes con una corrección sin probar cada uno.
- Convertir una corrección puntual en una reescritura amplia.
- Omitir la actualización de guía, contexto o el estado del reporte.
- Afirmar que un hallazgo de seguridad está resuelto sin evidencia de prueba.

