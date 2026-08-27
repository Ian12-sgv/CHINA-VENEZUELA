# Reportes de errores de QA y Testing

Esta carpeta contiene únicamente hallazgos reales, reproducibles y con evidencia. No crear reportes ficticios ni registrar secretos.

## Nombres de archivo

```text
error-qa-AAAA-MM-DD-HHMM-descripcion-corta.md
error-testing-AAAA-MM-DD-HHMM-descripcion-corta.md
```

- Usar `error-qa-` para hallazgos funcionales, interfaz, permisos, experiencia o aceptación.
- Usar `error-testing-` para fallas técnicas, API, base de datos, automatización, seguridad o integración.

## Plantilla obligatoria

```md
# [CRITICO|ALTO|MEDIO|BAJO] Título breve

- ID: QA-AAAA-MM-DD-001
- Tipo: QA | Testing
- Estado: Abierto | En corrección | Verificado | Cerrado
- Fecha: AAAA-MM-DD HH:MM (UTC-4)
- Módulo: Compras | Pedidos | Usuarios | API | Base de datos | Otro
- Entorno: Local | Pruebas | Producción autorizada
- Reportado por: qa-tester

## Descripción

Explicar el comportamiento incorrecto y el impacto.

## Pasos para reproducir

1. ...
2. ...
3. ...

## Resultado esperado

...

## Resultado actual

...

## Evidencia

- Comando, respuesta HTTP, log saneado o captura.
- No incluir secretos, contraseñas ni tokens.

## Severidad y riesgo

Justificar la clasificación y si bloquea la entrega.

## Corrección propuesta

...

## Verificación de corrección

- Fecha:
- Prueba repetida:
- Resultado:
- Responsable:
```

## Criterio de cierre

Un reporte solo puede cerrarse cuando la misma prueba que detectó el defecto pase y quede registrada la evidencia de verificación.

