---
name: entity-framework-core
description: Implementar persistencia, configuraciones, consultas y migraciones con EF Core y Npgsql. Usar al mapear entidades o modificar acceso a datos del backend.
---

# Entity Framework Core

## Objetivo

Implementar persistencia, configuraciones, consultas y migraciones con EF Core y Npgsql.

## Cuándo usarlo

Al mapear entidades o modificar acceso a datos del backend.

## Entradas esperadas

Entidad o caso de uso, esquema, convenciones, transacciones y consultas necesarias.

## Salida esperada

Fluent API, DbContext, migración y consultas o repositorios requeridos.

## Reglas que debe respetar

Configurar restricciones explícitas; no exponer entidades EF como DTOs; revisar transacciones. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Editar migraciones aplicadas, provocar N+1 o usar Include indiscriminadamente.

