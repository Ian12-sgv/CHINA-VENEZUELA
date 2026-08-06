---
name: disenar-base-datos-postgresql
description: Modelar datos consistentes, relacionales y trazables. Usar antes de crear o cambiar entidades, tablas, relaciones, índices o migraciones.
---

# Diseño de base de datos PostgreSQL

## Objetivo

Modelar datos consistentes, relacionales y trazables.

## Cuándo usarlo

Antes de crear o cambiar entidades, tablas, relaciones, índices o migraciones.

## Entradas esperadas

Proceso de negocio, entidades, consultas esperadas, reglas de retención y esquema actual.

## Salida esperada

Tablas con propósito, PK/FK, cardinalidades, restricciones, índices y plan de migración.

## Reglas que debe respetar

Usar snake_case, id y fechas UTC; registrar cambios mediante migraciones revisables. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Usar flotantes para dinero, relaciones sin FK, índices aleatorios o cambios sin migración.

