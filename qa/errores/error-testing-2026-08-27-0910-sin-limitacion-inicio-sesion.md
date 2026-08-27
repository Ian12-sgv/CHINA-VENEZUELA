# [ALTO] El inicio de sesión no limita intentos de autenticación

- ID: TEST-2026-08-27-001
- Tipo: Testing / Seguridad
- Estado: Abierto
- Fecha: 2026-08-27 09:10 (UTC-4)
- Módulo: Autenticación / API
- Entorno: Revisión estática local
- Reportado por: qa-tester

## Descripción

La ruta pública `POST /api/auth/iniciar-sesion` no tiene limitación de frecuencia, bloqueo temporal ni mecanismo equivalente contra intentos repetidos. Esto permite ataques de fuerza bruta contra las credenciales de usuarios si la API es accesible públicamente.

## Pasos para reproducir

1. Revisar `src/ChinaVenezuela.Api/Program.cs`.
2. Confirmar que se configura autenticación y autorización, pero no existen `AddRateLimiter`, `UseRateLimiter` ni una política de tasa.
3. Revisar `src/ChinaVenezuela.Api/Controllers/AuthController.cs`.
4. Confirmar que `POST /api/auth/iniciar-sesion` está marcado como `AllowAnonymous` y no aplica un límite propio.

## Resultado esperado

El endpoint de inicio de sesión debe limitar intentos por IP y/o usuario, devolver `429 Too Many Requests` al superar el umbral y registrar el evento sin exponer credenciales.

## Resultado actual

No existe una barrera de frecuencia implementada en la aplicación para esta ruta.

## Evidencia

- `Program.cs`: no registra ni habilita el middleware de rate limiting.
- `AuthController.cs`: ruta pública de autenticación sin política de límite.
- No se realizaron ataques ni intentos de fuerza bruta contra producción.

## Severidad y riesgo

**Alto.** Un atacante puede automatizar intentos de contraseña contra un endpoint expuesto públicamente. El riesgo aumenta para contraseñas reutilizadas o débiles.

## Corrección propuesta

1. Agregar `builder.Services.AddRateLimiter(...)` con una política específica para login.
2. Aplicar `app.UseRateLimiter()`.
3. Limitar por IP y, cuando sea posible, por nombre de usuario normalizado.
4. Devolver 429 al exceder el límite y registrar el intento.
5. Añadir pruebas de integración para los límites y su reinicio temporal.

## Verificación de corrección

- Fecha: Pendiente.
- Prueba repetida: Pendiente.
- Resultado: Pendiente.
- Responsable: Pendiente.

