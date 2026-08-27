# BAJO Documentación técnica conserva el formato anterior de correlativo

- ID: QA-2026-08-27-001
- Tipo: QA
- Estado: Abierto
- Fecha: 2026-08-27 14:26 (UTC-4)
- Módulo: Pedidos / Documentación
- Entorno: Local
- Reportado por: qa-tester

## Descripción

La aplicación crea grupos nuevos con el formato correlativo `001`, `002`, etc.; sin embargo, `contexto.md` aún describe el formato anterior `Pedido 0001`, `Pedido 0002`.

## Pasos para reproducir

1. Revisar la regla de grupos nuevos en `contexto.md`.
2. Revisar `PedidosService.ExtraerCorrelativo` y la creación de `Pedido`.

## Resultado esperado

La documentación técnica y la regla implementada deben describir el mismo formato: `001`, `002`, etc.

## Resultado actual

La aplicación usa solo tres dígitos, mientras la documentación técnica conserva el prefijo `Pedido` y cuatro dígitos.

## Evidencia

- `PedidosService` crea el nombre con `correlativo.ToString("D3")`.
- `contexto.md` contiene la frase `Pedido 0001, Pedido 0002, etc.`

## Severidad y riesgo

**Bajo.** No afecta la persistencia ni el funcionamiento, pero puede causar confusión operativa y desarrollo inconsistente.

## Corrección propuesta

Actualizar la regla correspondiente en `contexto.md` para usar `001`, `002`, etc.

## Verificación de corrección

- Fecha:
- Prueba repetida:
- Resultado:
- Responsable:
