---
name: auditoria-operativa
description: Registrar trazabilidad de acciones y transiciones de negocio relevantes. Usar cuando una operación crea, modifica, aprueba, rechaza, elimina o cambia estados.
---

# Auditoría operativa

## Objetivo

Registrar trazabilidad de acciones y transiciones de negocio relevantes.

## Cuándo usarlo

Cuando una operación crea, modifica, aprueba, rechaza, elimina o cambia estados.

## Entradas esperadas

Operación, entidad, actor, datos sensibles y reglas de retención.

## Salida esperada

Evento auditable, campos, integración transversal y consulta segura.

## Reglas que debe respetar

Registrar actor, fecha UTC, acción, entidad e identificador; no almacenar secretos ni contenido de archivos. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Hacer auditoría editable, omitir cambios administrativos o registrar contraseñas y tokens.

