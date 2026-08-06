# Arquitectura general

Aplicar un monolito modular con MVC y una Clean Architecture ligera. Cada módulo delimita su dominio, casos de uso, contratos y persistencia; se evitan dependencias directas entre interfaces y detalles de infraestructura.

Capas orientativas: API/MVC, aplicación (casos de uso y DTOs), dominio (entidades y reglas), e infraestructura (EF Core, archivos y servicios externos). Usar validaciones, Repository Pattern cuando aporte valor y auditoría transversal desde el inicio.

No crear microservicios en el MVP. Evaluarlos únicamente ante una necesidad operacional o de escala demostrable.
