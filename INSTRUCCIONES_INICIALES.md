# Sistema de Compras y Envíos China a Venezuela

Este repositorio conserva la memoria técnica y las reglas de trabajo del sistema interno. Antes de modificar código o proponer una solución en un chat nuevo, leer en este orden:

1. [`contexto/vision_general.md`](contexto/vision_general.md)
2. [`memoria/estado_actual.md`](memoria/estado_actual.md)
3. [`memoria/decisiones_tecnicas.md`](memoria/decisiones_tecnicas.md)
4. [`reglas/alcance_cerrado.md`](reglas/alcance_cerrado.md)
5. [`reglas/forma_de_trabajo.md`](reglas/forma_de_trabajo.md)

Después, revisar solo los documentos y skills pertinentes a la solicitud. Confirmar el alcance entendido y trabajar exclusivamente sobre él. Al cerrar una tarea, actualizar `memoria/estado_actual.md`, `memoria/historial_de_cambios.md` y cualquier decisión afectada.

Los skills de `skills/` son guías especializadas: invocar el que corresponda y leer su `SKILL.md` antes de ejecutar la tarea. Los skills no sustituyen las reglas de `reglas/`.

## Mantenimiento

- Registrar decisiones confirmadas, no hipótesis, en `memoria/decisiones_tecnicas.md`.
- Mantener pendientes con responsable o condición de cierre en `memoria/decisiones_pendientes.md`.
- Actualizar el estado y próximos pasos al finalizar cada entrega.
- Añadir un skill nuevo solo si cubre una tarea repetible; usar `skills/_plantilla_skill.md` como estructura y crear su carpeta con `SKILL.md`.
