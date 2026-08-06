---
name: revision-de-codigo
description: Detectar defectos accionables en cambios de código con foco en alcance y seguridad. Usar al revisar diffs, pull requests, implementaciones o correcciones propuestas.
---

# Revisión de código

## Objetivo

Detectar defectos accionables en cambios de código con foco en alcance y seguridad.

## Cuándo usarlo

Al revisar diffs, pull requests, implementaciones o correcciones propuestas.

## Entradas esperadas

Diff, solicitud original, archivos relacionados, convenciones y resultados de validación.

## Salida esperada

Hallazgos por severidad, ubicación, impacto, corrección concreta y límites de revisión.

## Reglas que debe respetar

Basar hallazgos en evidencia; contrastar con alcance; no editar salvo solicitud expresa. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Inventar defectos, comentar estilo sin impacto o revisar fuera del diff sin necesidad.

