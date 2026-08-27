# [MEDIO] Las operaciones de imágenes no cierran la sesión al recibir 401

- ID: TEST-2026-08-27-003
- Tipo: Testing / Integración frontend-API
- Estado: Abierto
- Fecha: 2026-08-27 09:12 (UTC-4)
- Módulo: Pedidos / Imágenes / Autenticación
- Entorno: Revisión estática local
- Reportado por: qa-tester

## Descripción

El cliente HTTP general procesa una respuesta 401 eliminando la sesión y notificando `sesion-invalida`. Las funciones específicas de imágenes (`requestMultipart` y `obtenerImagen`) no aplican esa lógica. Si el JWT expira mientras el usuario carga o consulta una imagen, la interfaz muestra un error pero conserva una sesión inválida.

## Pasos para reproducir

1. Iniciar sesión y esperar que el JWT expire, o usar un token inválido en `sessionStorage` dentro de un entorno de pruebas.
2. Intentar cargar, actualizar o consultar una imagen de pedido.
3. La API responde 401.
4. `requestMultipart` u `obtenerImagen` lanzan un error sin llamar a `limpiarSesion()` ni emitir `sesion-invalida`.

## Resultado esperado

Todas las llamadas autenticadas, incluidas las de carga y lectura de imágenes, deben limpiar la sesión y redirigir al inicio de sesión al recibir 401.

## Resultado actual

Solo la función `request` procesa el 401 de forma uniforme; las rutas de imágenes no lo hacen.

## Evidencia

- `frontend/src/api.ts`, función `request`: maneja `response.status === 401`.
- `frontend/src/api.ts`, funciones `requestMultipart` y `obtenerImagen`: no tienen manejo equivalente para 401.

## Severidad y riesgo

**Medio.** Genera un estado de interfaz incoherente y obliga al usuario a recargar o cerrar sesión manualmente después de expirar el token.

## Corrección propuesta

Extraer el manejo de 401 a una función común y aplicarla a `request`, `requestMultipart` y `obtenerImagen`. Añadir una prueba de cliente o integración que simule 401 para cada variante.

## Verificación de corrección

- Fecha: Pendiente.
- Prueba repetida: Pendiente.
- Resultado: Pendiente.
- Responsable: Pendiente.

