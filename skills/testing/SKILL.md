---
name: testing
description: Demostrar el comportamiento solicitado mediante pruebas proporcionales al riesgo. Usar antes de entregar cambios de lógica, api, persistencia, seguridad o interfaz.
---

# Testing

## Objetivo

Demostrar el comportamiento solicitado mediante pruebas proporcionales al riesgo.

## Cuándo usarlo

Antes de entregar cambios de lógica, API, persistencia, seguridad o interfaz.

## Entradas esperadas

Cambio, criterios de aceptación, reglas, pruebas existentes y entorno disponible.

## Salida esperada

Casos cubiertos, código de pruebas, comando ejecutado y resultado.

## Reglas que debe respetar

Probar reglas y bordes críticos; usar integración si aporta confianza; declarar lo no verificado. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Crear pruebas frágiles, depender del orden o afirmar validación no ejecutada.

