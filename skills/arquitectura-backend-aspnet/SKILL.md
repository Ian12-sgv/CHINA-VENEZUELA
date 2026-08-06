---
name: arquitectura-backend-aspnet
description: Diseñar cambios backend modulares aplicando MVC, SOLID y Clean Architecture ligera. Usar al definir estructura de solución, módulos, casos de uso, dtos o dependencias.
---

# Arquitectura backend ASP.NET Core

## Objetivo

Diseñar cambios backend modulares aplicando MVC, SOLID y Clean Architecture ligera.

## Cuándo usarlo

Al definir estructura de solución, módulos, casos de uso, DTOs o dependencias.

## Entradas esperadas

Caso de uso, módulos existentes, contratos, restricciones y código afectado.

## Salida esperada

Diseño o código completo por archivos, dependencias, validaciones y pruebas necesarias.

## Reglas que debe respetar

Mantener controladores delgados; aislar infraestructura; no crear microservicios sin autorización. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Poner negocio en controladores, generar dependencias circulares o refactorizar sin pedirlo.

