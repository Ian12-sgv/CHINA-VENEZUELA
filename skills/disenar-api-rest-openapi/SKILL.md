---
name: disenar-api-rest-openapi
description: Diseñar contratos REST seguros, claros y documentados para web y móvil. Usar al crear o cambiar rutas, requests, responses, errores, paginación o documentación.
---

# API REST y OpenAPI

## Objetivo

Diseñar contratos REST seguros, claros y documentados para web y móvil.

## Cuándo usarlo

Al crear o cambiar rutas, requests, responses, errores, paginación o documentación.

## Entradas esperadas

Caso de uso, actores, reglas, recursos existentes y formato de errores.

## Salida esperada

Ruta, método, autorización, DTOs, códigos HTTP, validaciones y definición OpenAPI.

## Reglas que debe respetar

Mantener compatibilidad salvo autorización; documentar errores y ejemplos; usar recursos plurales. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Exponer entidades, usar 200 para fallos o introducir cambios rompientes silenciosos.

