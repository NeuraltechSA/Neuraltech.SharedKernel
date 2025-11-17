# Instrucciones Copilot Unificadas

---

## 1. Instrucciones para la Capa de API / Presentación

# Instrucciones Copilot para la Capa de API / Presentación

## 1. Principios Generales

- **Basarse siempre en el código ya existente en el proyecto, especialmente en la estructura y convenciones de los módulos y de la carpeta SharedKernel (global o local).**
- La capa API expone la aplicación al exterior (REST, gRPC, GraphQL, etc.) y orquesta la interacción con Application.
- No debe contener lógica de dominio ni de infraestructura.
- Los controladores, handlers o endpoints deben ser delgados y delegar la lógica a los casos de uso de Application.
- Puede existir una carpeta SharedKernel local para utilidades, filtros, middlewares o contratos compartidos solo en la API.

## 2. Estructura de Carpetas

Organiza la capa API siguiendo estos lineamientos:

- **[Modulo]/**: Carpeta raíz para cada agregado/módulo (ej: UnidadesProduccion/).
  - **Handlers/**: Controladores, endpoints o handlers HTTP/gRPC/etc.
  - **DTO/**: Objetos de transferencia para requests y responses expuestos por la API.
  - **Routes/**: Definición de rutas o endpoints (opcional).
  - **Validators/**: Validaciones específicas de la API (ej: FluentValidation).
  - **Extensions/**: Métodos de extensión para la API (opcional).
- **SharedKernel/**: Utilidades, middlewares, filtros, atributos, etc. compartidos solo en la API.

Ejemplo:
```
API/
  ??? UnidadesProduccion/
  ?   ??? Handlers/
  ?   ??? DTO/
  ?   ??? Routes/
  ?   ??? Validators/
  ?   ??? Extensions/
  ??? SharedKernel/
  ?   ??? Filters/
  ?   ??? Middlewares/
  ?   ??? Attributes/
```

## 3. Nomenclatura de Archivos y Clases

- **Handlers/Controladores**: PascalCase, terminado en `Handler` o `Controller`.  
  Ej: `UnidadProduccionPOSTCreateHandler.cs`, `UnidadProduccionController.cs`
- **DTOs**: PascalCase, terminado en `RequestDTO` o `ResponseDTO`.  
  Ej: `UnidadProduccionCreateRequestDTO.cs`, `UnidadProduccionResponseDTO.cs`
- **Rutas**: PascalCase, terminado en `Routes`.  
  Ej: `UnidadesProduccionRoutes.cs`
- **Validadores**: PascalCase, terminado en `Validator`.  
  Ej: `UnidadProduccionCreateRequestValidator.cs`
- **Filtros/Middlewares/Atributos**: PascalCase, terminado en el propósito.  
  Ej: `ExceptionHandlingMiddleware.cs`, `AuthorizeAttribute.cs`

## 4. Convenciones de Código

- Los handlers/controladores deben ser delgados y delegar la lógica a los casos de uso de Application.
- Usar atributos de routing y validación estándar de ASP.NET Core o el framework correspondiente.
- Inyectar dependencias por constructor.
- No incluir lógica de negocio ni acceso a infraestructura directamente.
- Usar DTOs para requests y responses.
- Seguir la estructura y estilo de los módulos y SharedKernel local para nuevos endpoints.

## 5. Buenas Prácticas

- Mantener los handlers/controladores pequeños y enfocados en una sola responsabilidad.
- Reutilizar filtros, middlewares y utilidades de la carpeta SharedKernel local antes de recurrir a globales.
- Los nombres deben ser claros, descriptivos y en español (siguiendo el estándar del proyecto).
- **Siempre revisa y respeta el estilo y patrones ya implementados en el código existente, especialmente en los módulos y en cualquier carpeta SharedKernel local.**

---

**Estas instrucciones aseguran consistencia, mantenibilidad y alineación con la arquitectura existente, los módulos y la posibilidad de SharedKernel local por capa o proyecto.**

---

## 2. Instrucciones para la Capa de Aplicación

# Instrucciones Copilot para la Capa de Aplicación

## 1. Principios Generales

- **Basarse siempre en el código ya existente en el proyecto.**
- Mantener la separación de responsabilidades: la capa de aplicación orquesta casos de uso, no contiene lógica de dominio ni de infraestructura.
- Usar DTOs para requests y responses.
- Los casos de uso deben ser reutilizables y fácilmente testeables.

## 2. Estructura de Carpetas

Organiza la capa de aplicación siguiendo estos lineamientos:

- **UseCases/**: Un subdirectorio por agregado o módulo, y dentro de cada uno, un subdirectorio por caso de uso (Create, Update, Delete, Paginate, etc.).
- **DTO/**: Objetos de transferencia de datos para requests y responses (pueden estar junto a los casos de uso o en una carpeta aparte si son compartidos).

Ejemplo:
```
Application/
  ??? UnidadesProduccion/
      ??? UseCases/
          ??? CreateUnidadProduccion/
          ?   ??? CreateUnidadProduccionUseCase.cs
          ?   ??? CreateUnidadProduccionDTO.cs
          ??? UpdateUnidadProduccion/
          ?   ??? UpdateUnidadProduccionUseCase.cs
          ?   ??? UpdateUnidadProduccionDTO.cs
          ??? DeleteUnidadProduccion/
          ?   ??? DeleteUnidadProduccionUseCase.cs
          ??? GetUnidadProduccionById/
              ??? GetUnidadProduccionByIdUseCase.cs
              ??? GetUnidadProduccionByIdResponseDTO.cs
```

## 3. Nomenclatura de Archivos y Clases

- **Casos de Uso**: PascalCase, terminado en `UseCase`.  
  Ej: `CreateUnidadProduccionUseCase.cs`
- **DTOs**: PascalCase, terminado en `DTO`.  
  Ej: `CreateUnidadProduccionDTO.cs`, `GetUnidadProduccionByIdResponseDTO.cs`

## 4. Convenciones de Código

- Los casos de uso heredan de clases base del SharedKernel (`CreateUseCase`, `UpdateUseCase`, `DeleteUseCase`, etc.).
- Inyectar dependencias por constructor (repositorios, unit of work, event bus, logger, etc.).
- No incluir lógica de dominio ni acceso a infraestructura directamente.
- Los métodos principales deben ser asíncronos y devolver Task o Task<T>.
- Usar métodos protegidos para mapear entidades y DTOs.
- Manejar excepciones de dominio y devolver resultados claros.

## 5. Ejemplo de Caso de Uso con Response

```
// Application/UnidadesProduccion/UseCases/GetUnidadProduccionById/GetUnidadProduccionByIdUseCase.cs
using Infracsoft.Importacion.Domain.UnidadesProduccion.Contracts;
using Infracsoft.Importacion.Domain.UnidadesProduccion.Entities;
using Neuraltech.SharedKernel.Application.UseCases.Base;

namespace Infracsoft.Importacion.Application.UnidadesProduccion.UseCases.GetUnidadProduccionById
{
    public class GetUnidadProduccionByIdUseCase : BaseUseCase<Guid, GetUnidadProduccionByIdResponseDTO>
    {
        private readonly IUnidadProduccionRepository _repository;

        public GetUnidadProduccionByIdUseCase(IUnidadProduccionRepository repository)
        {
            _repository = repository;
        }

        protected override async Task<UseCaseResponse<GetUnidadProduccionByIdResponseDTO>> ExecuteLogic(Guid id)
        {
            var entity = await _repository.Find(id);
            if (entity == null)
                return UseCaseResponse.Fail<GetUnidadProduccionByIdResponseDTO>("No se encontró la unidad de producción.");

            var response = new GetUnidadProduccionByIdResponseDTO
            {
                Id = entity.Id.Value,
                Nombre = entity.Nombre.Value
            };
            return UseCaseResponse.Success(response);
        }
    }
}

// Application/UnidadesProduccion/UseCases/GetUnidadProduccionById/GetUnidadProduccionByIdResponseDTO.cs
namespace Infracsoft.Importacion.Application.UnidadesProduccion.UseCases.GetUnidadProduccionById
{
    public class GetUnidadProduccionByIdResponseDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
```

## 6. Buenas Prácticas

- Mantener los casos de uso pequeños y enfocados en una sola responsabilidad.
- Reutilizar lógica de dominio a través de servicios de dominio.
- Los nombres deben ser claros, descriptivos y en español (siguiendo el estándar del proyecto).
- **Siempre revisa y respeta el estilo y patrones ya implementados en el código existente.**

---

**Estas instrucciones aseguran consistencia, mantenibilidad y alineación con la arquitectura existente.**

---

## 3. Instrucciones para la Capa de Dominio

# Instrucciones Copilot para la Capa de Dominio

## 1. Estructura de Carpetas

Organiza la capa de dominio siguiendo estos lineamientos y **basándote siempre en el código ya existente en el proyecto**:

- **Entities/**: Clases de entidades de dominio (heredan de `AggregateRoot` o `Entity`).
- **ValueObjects/**: Objetos de valor inmutables y validados.
- **Events/**: Eventos de dominio (heredan de `BaseEvent`).
- **Contracts/**: Interfaces de repositorios y servicios de dominio.
- **Criteria/**: Objetos para criterios de búsqueda y filtrado.
- **Exceptions/**: Excepciones específicas del dominio (opcional, si aplica).
- **Services/**: Servicios de dominio para lógica reutilizable entre casos de uso.

Ejemplo:
```
Domain/
  ??? UnidadesProduccion/
      ??? Entities/
      ??? ValueObjects/
      ??? Events/
      ??? Contracts/
      ??? Criteria/
      ??? Exceptions/
      ??? Services/
```

## 2. Nomenclatura de Archivos y Clases

- **Entidades**: Singular, PascalCase.  
  Ej: `UnidadProduccion.cs` ? `UnidadProduccion : AggregateRoot`
- **Objetos de Valor**: Singular, PascalCase, terminados en el concepto.  
  Ej: `UnidadProduccionNombre.cs` ? `UnidadProduccionNombre : ValueObject`
- **Eventos**: PascalCase, terminado en `Event`.  
  Ej: `UnidadProduccionCreatedEvent.cs`
- **Interfaces**: Prefijo `I`, PascalCase, terminado en el propósito.  
  Ej: `IUnidadProduccionRepository.cs`
- **Criterios**: PascalCase, terminado en `Criteria`.  
  Ej: `UnidadProduccionCriteria.cs`
- **Excepciones**: PascalCase, terminado en `Exception`.  
  Ej: `UnidadProduccionNotFoundException.cs`
- **Servicios de Dominio**: PascalCase, terminado en `Service`.  
  Ej: `UnidadProduccionValidatorService.cs`

## 3. Convenciones de Código

- **Entidades**:
  - Heredan de `AggregateRoot` o `Entity` del SharedKernel.
  - Tienen propiedades inmutables (preferir `init` o `private set`).
  - Métodos de fábrica estáticos para creación controlada.
  - Métodos para registrar eventos de dominio (`RecordDomainEvent`).

- **Objetos de Valor**:
  - Inmutables.
  - Validan sus invariantes en el constructor.
  - Pueden lanzar excepciones de dominio si el valor es inválido.

- **Eventos**:
  - Heredan de `BaseEvent`.
  - Usar `record` para inmutabilidad.
  - Definir propiedades relevantes y el `MessageName`.

- **Interfaces de Repositorio**:
  - Heredan de interfaces genéricas del SharedKernel si aplica.
  - Definir solo métodos necesarios para el agregado.

- **Criterios**:
  - Heredan de `BaseCriteria<T>`.
  - Usar para encapsular filtros, órdenes y paginación.

- **Servicios de Dominio**:
  - Encapsulan lógica de negocio reutilizable entre casos de uso.
  - No dependen de infraestructura ni de servicios externos.
  - Basarse en la estructura y convenciones ya presentes en el código existente.

## 4. Ejemplo de Entidad

```
Domain/UnidadesProduccion/Entities/UnidadProduccion.cs
using Infracsoft.Importacion.Domain.UnidadesProduccion.ValueObjects;
using Neuraltech.SharedKernel.Domain.Base;
using Infracsoft.Importacion.Domain.UnidadesProduccion.Events;

namespace Infracsoft.Importacion.Domain.UnidadesProduccion.Entities
{
    public class UnidadProduccion : AggregateRoot
    {
        public UnidadProduccionNombre Nombre { get; init; }

        public UnidadProduccion(Guid id, string nombre) : base(new UnidadProduccionId(id))
        {
            Nombre = new UnidadProduccionNombre(nombre);
        }

        public static UnidadProduccion Create(Guid id, string nombre)
        {
            var unidad = new UnidadProduccion(id, nombre);
            unidad.RecordDomainEvent(new UnidadProduccionCreatedEvent
            {
                UnidadProduccionId = unidad.Id.Value,
                Nombre = unidad.Nombre.Value
            });
            return unidad;
        }
    }
}
```

## 5. Buenas Prácticas

- Mantén la lógica de negocio y validaciones en la capa de dominio.
- No incluyas dependencias de infraestructura ni lógica de acceso a datos.
- Usa excepciones de dominio para invariantes y errores de negocio.
- Los nombres deben ser claros, descriptivos y en español (siguiendo el estándar del proyecto).
- **Siempre revisa y respeta el estilo y patrones ya implementados en el código existente.**

---

**Estas instrucciones aseguran consistencia, mantenibilidad y alineación con la arquitectura existente.**

---

## 4. Instrucciones para la Capa de Infraestructura

# Instrucciones Copilot para la Capa de Infraestructura

## 1. Principios Generales

- **Basarse siempre en el código ya existente en el proyecto, especialmente en la estructura y convenciones del módulo UnidadesProduccion.**
- La infraestructura implementa detalles técnicos: persistencia, integración con servicios externos, colas, correo, logging, etc.
- No debe contener lógica de dominio ni de aplicación.
- Los servicios y repositorios de infraestructura implementan interfaces definidas en Domain o Application.

## 2. Estructura de Carpetas

Organiza la capa de infraestructura siguiendo estos lineamientos y tomando como referencia la organización del módulo UnidadesProduccion:

- **[Modulo]/**: Carpeta raíz para cada agregado/módulo (ej: UnidadesProduccion/).
  - **Models/**: Modelos de persistencia (entidades de base de datos, mapeos ORM, etc.).
  - **Services/**: Servicios técnicos o de integración (por ejemplo, parsers, adaptadores, etc.).
  - **Repositories/**: Implementaciones concretas de los repositorios de dominio.
  - **DTO/**: Objetos de transferencia específicos de infraestructura (si aplica).
- **SharedKernel/**: Servicios, utilidades o configuraciones compartidas entre módulos.

Ejemplo:
```
Infraestructure/
  ??? UnidadesProduccion/
  ?   ??? Models/
  ?   ??? Services/
  ?   ??? Repositories/
  ?   ??? DTO/
  ??? SharedKernel/
  ?   ??? Services/
```

## 3. Nomenclatura de Archivos y Clases

- **Repositorios**: PascalCase, terminado en `Repository`.  
  Ej: `UnidadProduccionRepository.cs`
- **Servicios**: PascalCase, terminado en `Service` o el propósito técnico.  
  Ej: `UnidadProduccionModelParser.cs`
- **Modelos**: PascalCase, singular, representando la tabla o colección.  
  Ej: `UnidadProduccion.cs`
- **Clases de contexto**: PascalCase, terminado en `DbContext` o similar.  
  Ej: `ImportacionDbContext.cs`

## 4. Convenciones de Código

- Implementar interfaces de dominio o aplicación.
- Inyectar dependencias por constructor.
- No incluir lógica de negocio, solo detalles técnicos.
- Usar patrones como Repository, Adapter, Decorator, etc., según corresponda.
- Configurar la inyección de dependencias en un único lugar (por ejemplo, en `Startup` o `Program`).
- Seguir la estructura y estilo de UnidadesProduccion para nuevos módulos.

## 5. Buenas Prácticas

- Mantener la infraestructura desacoplada del dominio y la aplicación.
- Reutilizar servicios y utilidades técnicas cuando sea posible.
- Los nombres deben ser claros, descriptivos y en español (siguiendo el estándar del proyecto).
- **Siempre revisa y respeta el estilo y patrones ya implementados en el código existente, especialmente en UnidadesProduccion.**

---

**Estas instrucciones aseguran consistencia, mantenibilidad y alineación con la arquitectura existente y el módulo UnidadesProduccion.**

