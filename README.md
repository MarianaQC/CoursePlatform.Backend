# 📚 Course Platform API

API REST para gestión de plataforma de cursos online, construida con .NET 8 y Clean Architecture.

## 🏗️ Arquitectura

El proyecto sigue los principios de **Clean Architecture** con las siguientes capas:

```text
CoursePlatform/
├── src/
│ ├── CoursePlatform.Domain/ # Entidades y reglas de negocio
│ ├── CoursePlatform.Application/ # Casos de uso y DTOs
│ ├── CoursePlatform.Infrastructure/ # Persistencia y servicios externos
│ └── CoursePlatform.Api/ # Controladores y configuración
└── tests/
└── CoursePlatform.Tests/ # Tests unitarios
```

## 🛠️ Tecnologías

- **.NET 8.0**
- **Entity Framework Core 8.x** con MySQL (Pomelo)
- **ASP.NET Core Identity** + JWT Bearer Authentication
- **FluentValidation** para validaciones
- **Swagger/OpenAPI** para documentación
- **xUnit** + **Moq** + **FluentAssertions** para testing
- **Docker** y **Docker Compose**

## 📋 Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0](https://dev.mysql.com/downloads/mysql/)
- [Docker](https://docs.docker.com/get-docker/) (opcional)
- [JetBrains Rider](https://www.jetbrains.com/rider/) o VS Code

## ⚙️ Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/CoursePlatform.git
cd CoursePlatform
```

### 2. Configurar la base de datos
Crear la base de datos en MySQL:

```bash
CREATE DATABASE CoursePlatform_DB;
```

### 3. Configurar la cadena de conexión

Editar src/CoursePlatform.Api/appsettings.json:

```bash
{
"ConnectionStrings": {
"DefaultConnection": "Server=localhost;Port=3306;Database=CoursePlatform_DB;User=root;Password=TU_PASSWORD;"
}
}
```

### 4. Aplicar migraciones

```bash
dotnet ef database update \
--project src/CoursePlatform.Infrastructure/CoursePlatform.Infrastructure.csproj \
--startup-project src/CoursePlatform.Api/CoursePlatform.Api.csproj
```

### 5. Ejecutar la aplicación

```bash
dotnet run --project src/CoursePlatform.Api/CoursePlatform.Api.csproj
```

La API estará disponible en:

   - HTTP: http://localhost:5000
   - Swagger: http://localhost:5000/swagger

# 🐳 Docker - Ejecutar con Docker Compose

También puedes ejecutar la aplicación usando Docker Compose:

```bash
# Construir e iniciar todos los servicios
docker-compose up -d

# Ver logs
docker-compose logs -f api

# Detener servicios
docker-compose down

# Detener y eliminar volúmenes
docker-compose down -v
```

### Construir solo la imagen

```bash
docker build -t courseplatform-api .
```

### 🧪 Ejecutar Tests

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar tests específicos
dotnet test --filter "FullyQualifiedName~CourseServiceTests"
```

---

## 📡 Endpoints de la API

### 🔑 Autenticación
| Método | Endpoint | Descripción | Auth |
| :--- | :--- | :--- | :--- |
| POST | `/api/auth/register` | Registrar nuevo usuario | ❌ |
| POST | `/api/auth/login` | Iniciar sesión | ❌ |

### 📚 Cursos
| Método | Endpoint | Descripción | Auth |
| :--- | :--- | :--- | :--- |
| GET | `/api/courses/search` | Buscar cursos con paginación | ✅ |
| GET | `/api/courses/{id}` | Obtener curso por ID | ✅ |
| GET | `/api/courses/{id}/summary` | Resumen con lecciones | ✅ |
| POST | `/api/courses` | Crear nuevo curso | ✅ |
| PUT | `/api/courses/{id}` | Actualizar curso | ✅ |
| PATCH | `/api/courses/{id}/publish` | Publicar curso | ✅ |
| PATCH | `/api/courses/{id}/unpublish` | Despublicar curso | ✅ |
| DELETE | `/api/courses/{id}` | Soft delete | ✅ |
| DELETE | `/api/courses/{id}/hard` | Hard delete (Solo Admin) | ✅ |

### 📖 Lecciones
| Método | Endpoint | Descripción | Auth |
| :--- | :--- | :--- | :--- |
| GET | `/api/lessons/course/{courseId}` | Listar lecciones de un curso | ✅ |
| GET | `/api/lessons/{id}` | Obtener lección por ID | ✅ |
| POST | `/api/lessons` | Crear nueva lección | ✅ |
| PUT | `/api/lessons/{id}` | Actualizar lección | ✅ |
| PATCH | `/api/lessons/{id}/move-up` | Subir posición de lección | ✅ |
| PATCH | `/api/lessons/{id}/move-down` | Bajar posición de lección | ✅ |
| POST | `/api/lessons/course/{courseId}/reorder` | Reordenamiento masivo | ✅ |
| DELETE | `/api/lessons/{id}` | Soft delete | ✅ |

---

## Autenticación
La API utiliza **JWT Bearer Tokens**. Para acceder a los endpoints protegidos:
1.  Regístrese o inicie sesión para obtener el token.
2.  Incluya el header en sus peticiones:
    `Authorization: Bearer {tu-token-jwt}`
---

## Credenciales de Prueba (Admin)
* **Email**: marianaqc64@gmail.com
* **Password**: osfigprczoilwdvw

---
## Reglas de Negocio Implementadas

1.  **Publicación Condicional**: Un curso solo puede pasar a estado `Published` si tiene al menos una lección activa.
2.  **Orden Dinámico**: Las lecciones mantienen un orden único por curso. Al mover una lección, se intercambia automáticamente con la adyacente.
3.  **Eliminación Lógica (Soft Delete)**: Las peticiones `DELETE` estándar marcan `IsDeleted = true`. Los datos permanecen en la DB pero son ignorados por la API.
4.  **Estandarización de Respuestas**: Todas las respuestas siguen la estructura genérica `ApiResponse<T>`.

---

# 🔧 Comandos Útiles


## Crear nueva migración

```bash
dotnet ef migrations add NombreMigracion \
--project src/CoursePlatform.Infrastructure/CoursePlatform.Infrastructure.csproj \
--startup-project src/CoursePlatform.Api/CoursePlatform.Api.csproj
```

## Aplicar migraciones
```bash
dotnet ef database update \
--project src/CoursePlatform.Infrastructure/CoursePlatform.Infrastructure.csproj \
--startup-project src/CoursePlatform.Api/CoursePlatform.Api.csproj
```

## Revertir última migración
```bash
dotnet ef migrations remove \
--project src/CoursePlatform.Infrastructure/CoursePlatform.Infrastructure.csproj \
--startup-project src/CoursePlatform.Api/CoursePlatform.Api.csproj
```

## Compilar solución
```bash
dotnet build
```

## Ejecutar tests

```bash
dotnet test
```

## Ejecutar API
```bash
dotnet run --project src/CoursePlatform.Api/CoursePlatform.Api.csproj
```

---

## 📂 Estructura del Proyecto

```text
CoursePlatform/
├── src/
│   ├── CoursePlatform.Domain/
│   │   ├── Entities/
│   │   │   ├── ApplicationUser.cs
│   │   │   ├── BaseEntity.cs
│   │   │   ├── Course.cs
│   │   │   └── Lesson.cs
│   │   ├── Enums/
│   │   │   └── CourseStatus.cs
│   │   ├── Interfaces/
│   │   │   ├── ICourseRepository.cs
│   │   │   ├── ILessonRepository.cs
│   │   │   ├── IRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   └── CoursePlatform.Domain.csproj
│   │
│   ├── CoursePlatform.Application/
│   │   ├── Common/
│   │   │   ├── Errors/
│   │   │   │   └── DomainErrors.cs
│   │   │   ├── ApiResponse.cs
│   │   │   ├── PagedResult.cs
│   │   │   └── Result.cs
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   │   ├── AuthResponse.cs
│   │   │   │   ├── LoginRequest.cs
│   │   │   │   └── RegisterRequest.cs
│   │   │   ├── Course/
│   │   │   │   ├── CourseResponse.cs
│   │   │   │   ├── CourseSearchRequest.cs
│   │   │   │   ├── CourseSummaryResponse.cs
│   │   │   │   ├── CreateCourseRequest.cs
│   │   │   │   └── UpdateCourseRequest.cs
│   │   │   └── Lesson/
│   │   │       ├── CreateLessonRequest.cs
│   │   │       ├── LessonResponse.cs
│   │   │       ├── ReorderLessonRequest.cs
│   │   │       └── UpdateLessonRequest.cs
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   ├── ICourseService.cs
│   │   │   ├── IJwtService.cs
│   │   │   └── ILessonService.cs
│   │   ├── Mappings/
│   │   │   └── MappingExtensions.cs
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   ├── CourseService.cs
│   │   │   ├── JwtService.cs
│   │   │   └── LessonService.cs
│   │   ├── Validators/
│   │   │   ├── Auth/
│   │   │   │   ├── LoginRequestValidator.cs
│   │   │   │   └── RegisterRequestValidator.cs
│   │   │   ├── Course/
│   │   │   │   ├── CreateCourseRequestValidator.cs
│   │   │   │   └── UpdateCourseRequestValidator.cs
│   │   │   └── Lesson/
│   │   │       ├── CreateLessonRequestValidator.cs
│   │   │       └── UpdateLessonRequestValidator.cs
│   │   └── CoursePlatform.Application.csproj
│   │
│   ├── CoursePlatform.Infrastructure/
│   │   ├── Data/
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Migrations/
│   │   │   ├── XXXXXXXXXXXXXX_InitialCreate.cs
│   │   │   └── ApplicationDbContextModelSnapshot.cs
│   │   ├── Repositories/
│   │   │   ├── CourseRepository.cs
│   │   │   ├── LessonRepository.cs
│   │   │   ├── Repository.cs
│   │   │   └── UnitOfWork.cs
│   │   ├── Seed/
│   │   │   └── DataSeeder.cs
│   │   └── CoursePlatform.Infrastructure.csproj
│   │
│   └── CoursePlatform.Api/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── CoursesController.cs
│       │   └── LessonsController.cs
│       ├── Extensions/
│       │   ├── ApplicationBuilderExtensions.cs
│       │   └── ServiceCollectionExtensions.cs
│       ├── Middleware/
│       │   └── ExceptionHandlingMiddleware.cs
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── appsettings.Development.json
│       ├── appsettings.json
│       ├── CoursePlatform.Api.csproj
│       └── Program.cs
│
├── tests/
│   └── CoursePlatform.Tests/
│       ├── Mocks/
│       │   └── MockRepositoryFactory.cs
│       ├── Unit/
│       │   ├── CourseServiceTests.cs
│       │   └── LessonServiceTests.cs
│       └── CoursePlatform.Tests.csproj
│
├── .dockerignore
├── .gitignore
├── CoursePlatform.sln
├── docker-compose.yml
├── Dockerfile
└── README.md
```

# Propietario
- Mariana Quintero Cardona 
- Email: quinteromc.098@gmail.com
- GitHub: https://github.com/MarianaQC
- Clan: Hopper