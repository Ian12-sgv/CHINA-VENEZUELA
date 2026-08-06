---
name: gestionar-archivos-y-fotos
description: Gestionar evidencia operativa con almacenamiento y acceso seguro. Usar al implementar carga, descarga, vinculación, validación o retención de fotos y documentos.
---

# Archivos y fotos

## Objetivo

Gestionar evidencia operativa con almacenamiento y acceso seguro.

## Cuándo usarlo

Al implementar carga, descarga, vinculación, validación o retención de fotos y documentos.

## Entradas esperadas

Tipo de archivo, propietario de negocio, tamaños, almacenamiento, permisos y retención.

## Salida esperada

Contrato de carga, metadatos, autorización y estrategia de almacenamiento.

## Reglas que debe respetar

Validar tamaño, extensión, MIME y contenido; usar nombres no predecibles; auditar acciones relevantes. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Confiar en el nombre del cliente, usar rutas públicas sin control o sobrescribir evidencia.

