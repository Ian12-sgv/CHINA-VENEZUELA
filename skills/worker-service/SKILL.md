---
name: worker-service
description: Implementar tareas en segundo plano observables, controladas e idempotentes. Usar al procesar colas, archivos, importaciones, notificaciones o trabajo diferido.
---

# Worker Service

## Objetivo

Implementar tareas en segundo plano observables, controladas e idempotentes.

## Cuándo usarlo

Al procesar colas, archivos, importaciones, notificaciones o trabajo diferido.

## Entradas esperadas

Tarea, disparador, volumen, prioridad, transacción de origen y política de reintento.

## Salida esperada

Contrato, estado persistente, worker, reintentos, telemetría y pruebas.

## Reglas que debe respetar

Crear tareas tras confirmar transacción; limitar concurrencia; registrar inicio, fin y error. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Mantener tareas solo en memoria, reintentar sin límite o bloquear el apagado del servicio.

