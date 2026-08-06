---
name: autenticacion-y-roles
description: Aplicar autenticación JWT y autorización de mínimo privilegio con roles y policies. Usar al implementar usuarios, login, permisos, roles, claims o endpoints protegidos.
---

# Autenticación y roles

## Objetivo

Aplicar autenticación JWT y autorización de mínimo privilegio con roles y policies.

## Cuándo usarlo

Al implementar usuarios, login, permisos, roles, claims o endpoints protegidos.

## Entradas esperadas

Actores, operaciones, matriz de permisos, requisitos de sesión y mecanismos existentes.

## Salida esperada

Matriz de acceso o código con claims, policies, validaciones y pruebas.

## Reglas que debe respetar

Proteger el backend por policy; asociar actor a auditoría; mantener secretos fuera del repositorio. Cumplir siempre eglas/alcance_cerrado.md, preservar nombres y estructura existente, y pedir autorización antes de ampliar el alcance.

## Flujo de trabajo

1. Leer el contexto, estado, decisiones y reglas pertinentes.
2. Delimitar el cambio pedido y confirmar los supuestos necesarios.
3. Diseñar o implementar solo el resultado solicitado.
4. Validar proporcionalmente el resultado y registrar documentación si corresponde.

## Ejemplo de uso

$example

## Errores que debe evitar

Confiar en el frontend, usar tokens permanentes o guardar secretos en código.

