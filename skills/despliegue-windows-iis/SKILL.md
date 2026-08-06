---
name: despliegue-windows-iis
description: Preparar publicaciones ASP.NET Core repetibles, seguras y reversibles en IIS. Usar al publicar, configurar ambientes, certificados, pools, variables o rollback.
---

# Despliegue Windows Server e IIS

## Objetivo

Preparar publicaciones ASP.NET Core repetibles, seguras y reversibles en IIS.

## Cuándo usarlo

Al publicar, configurar ambientes, certificados, pools, variables o rollback.

## Entradas esperadas

Ambiente, versión, dominio, configuración, base de datos y ventana de cambio.

## Salida esperada

Checklist y runbook con publicación, respaldo, configuración, verificación y reversión.

## Reglas que debe respetar

Mantener secretos fuera del repositorio; aplicar mínimo privilegio; registrar versión desplegada. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Desplegar sin autorización, migrar sin revisión o no tener respaldos y rollback.

