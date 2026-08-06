---
name: sincronizacion-movil
description: Sincronizar cambios de móvil y API sin pérdidas ni duplicados. Usar al definir colas, reintentos, idempotencia, conflictos o subida de fotos.
---

# Sincronización móvil

## Objetivo

Sincronizar cambios de móvil y API sin pérdidas ni duplicados.

## Cuándo usarlo

Al definir colas, reintentos, idempotencia, conflictos o subida de fotos.

## Entradas esperadas

Operaciones locales, versiones, prioridades, conflictos esperados y límites de red.

## Salida esperada

Protocolo de sync, claves de idempotencia, dependencias, reintentos y resolución de conflictos.

## Reglas que debe respetar

Confirmar en servidor antes de borrar pendientes; conservar la cola tras reinicio; observar fallos. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Usar last-write-wins sin aprobación, reintentos infinitos o transacciones remotas gigantes.

