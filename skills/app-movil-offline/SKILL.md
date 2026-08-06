---
name: app-movil-offline
description: Diseñar trabajo móvil sin conectividad para inspecciones, fotos y escaneo. Usar en fase móvil, al definir almacenamiento local, cámara, escáner u operaciones sin red.
---

# App móvil offline

## Objetivo

Diseñar trabajo móvil sin conectividad para inspecciones, fotos y escaneo.

## Cuándo usarlo

En fase móvil, al definir almacenamiento local, cámara, escáner u operaciones sin red.

## Entradas esperadas

Flujo móvil, plataforma elegida, datos mínimos, seguridad y conectividad esperada.

## Salida esperada

Diseño local, cola de operaciones, estados de UI, retención y pruebas de desconexión.

## Reglas que debe respetar

Mantener tareas pequeñas e idempotentes; informar estado pendiente/sincronizado/error; cifrar datos sensibles. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Asumir conectividad, perder fotos pendientes o implementar antes de la fase autorizada.

