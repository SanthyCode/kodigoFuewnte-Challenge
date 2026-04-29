# Decisiones de Arquitectura - Kódigo Fuente Challenge

Este documento detalla las decisiones técnicas y arquitectónicas tomadas para la modernización del módulo de Separatas del sistema POS, priorizando la escalabilidad, el rendimiento y la mantenibilidad del código.

## 1. Patrones de Diseño y Principios SOLID
- **Clean Architecture & Dependency Inversion:** Se estructuró el backend en capas estrictas (Domain, Application, Infrastructure, API). Esto garantiza que la lógica de negocio central no dependa de frameworks externos ni de la base de datos, facilitando futuras migraciones o actualizaciones del ORM sin afectar el núcleo del sistema.
- **Strategy Pattern (Open/Closed Principle):** Para el motor de cálculo de promociones, se implementó el patrón Strategy mediante la interfaz `IPromotionStrategy`. Esto resuelve directamente el requerimiento de negocio de "añadir nuevos tipos de promociones en el futuro". Si mañana se requiere una promoción "2x1", se creará una nueva clase que implemente la interfaz, sin necesidad de modificar ni arriesgar el código existente del flujo de ventas.
- **Repository Pattern:** Abstrae la persistencia de datos. Permite aislar consultas complejas (como la validación de solapamiento de fechas por producto) fuera de los controladores y validadores, facilitando la inyección de dependencias y la creación de pruebas unitarias (Mocks).

## 2. Stack Tecnológico

### Backend (.NET 9 & C#)
- Se eligió el ecosistema .NET por su robustez en entornos transaccionales financieros (POS) y su tipado fuerte.
- Se implementó un motor de validación asíncrono para evaluar reglas de negocio complejas (cruces de fechas por ID de producto y reglas financieras de descuento) antes de interactuar con la base de datos, protegiendo la integridad transaccional.

### Frontend (React 18 + Redux Toolkit Query)
- **Gestión de Estado (RTK Query):** En lugar de manejar estados asíncronos frágiles con `useState` y `useEffect`, se delegó la capa de red a RTK Query. Esto centraliza las llamadas a la API, maneja automáticamente los estados de carga (`isLoading`), y prepara la aplicación para un manejo de caché eficiente a medida que el POS escale.
- **Técnica de Debouncing:** Se implementó un retraso intencional (500ms) en la validación en tiempo real del formulario para mitigar la sobrecarga del servidor, garantizando que solo se realicen consultas HTTP cuando el usuario hace una pausa en la escritura.

### Base de Datos (PostgreSQL)
- Base de datos relacional elegida por su cumplimiento estricto de principios ACID, fundamental en sistemas de punto de venta.
- Se utilizaron tipos de datos financieros precisos (`DECIMAL`) para evitar errores de coma flotante en el cálculo de descuentos, y `UUID` como llaves primarias para prevenir ataques de enumeración y facilitar la replicación distribuida de datos en el futuro.

## 3. Infraestructura y DevOps
- **Dockerización:** Todo el entorno (Frontend, Backend y Base de Datos) está orquestado mediante `docker-compose`. Esto elimina el problema de "funciona en mi máquina" y estandariza el entorno para cualquier desarrollador del equipo.
- **Integración Continua (GitHub Actions):** Se configuró un pipeline de CI/CD que automatiza la verificación de calidad estática (Linter), la ejecución de pruebas unitarias de la lógica de negocio y la publicación de imágenes en GitHub Packages (GHCR), alineándose con las prácticas modernas de entrega continua.