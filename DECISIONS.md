# Decisiones de Arquitectura - Kódigo Fuente Challenge

### 1. Stack Tecnológico
- **Frontend**: React 18 + Redux Toolkit (RTK Query). Se eligió RTK Query para gestionar el estado asíncrono y la validación en tiempo real sin saturar el componente.
- **Backend**: .NET 9. Se utilizó para aprovechar las nuevas funcionalidades de OpenAPI y el rendimiento superior del CLR.
- **Base de Datos**: PostgreSQL. Por su robustez con tipos de datos complejos como UUID y su facilidad de integración en contenedores.

### 2. Patrones de Diseño
- **Clean Architecture**: Separación de capas (Domain, Application, Infrastructure, API) para asegurar que las reglas de negocio no dependan de la base de datos o frameworks externos.
- **Strategy Pattern**: Implementado para el cálculo de promociones (Descuento Directo vs Porcentaje). Esto permite añadir nuevas promociones (ej. 2x1) creando una nueva clase sin tocar el código existente (Cumpliendo el principio Open/Closed).
- **Dependency Inversion**: Uso de interfaces para desacoplar las capas.

### 3. Infraestructura
- **Docker Compose**: Se configuró para garantizar que cualquier desarrollador pueda levantar el entorno completo (DB, API, Web) con un solo comando.