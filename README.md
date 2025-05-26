# Система анализа и хранения файлов
# Работа выполнена Гуршумовым Даниилом БПИ239
## Описание

**Система анализа файлов** — это микросервисное решение для загрузки, хранения, анализа и доставки текстовых файлов. Архитектура построена на принципах чистой архитектуры (Clean Architecture) и паттернах SOLID, что обеспечивает масштабируемость, тестируемость и простоту поддержки.

---

## Используемые технологии

- **.NET 8.0** (ASP.NET Core Web API)
- **PostgreSQL 15** (две отдельные БД для хранения и анализа)
- **Docker, Docker Compose** (оркестрация и деплой)
- **xUnit, Moq** (юнит-тестирование)
- **Swagger/OpenAPI** (документация API)
- **Entity Framework Core** (ORM, миграции)
- **SOLID, Clean Architecture, Dependency Injection, Repository Pattern**

---

## Архитектурные паттерны и принципы

- **Чистая архитектура (Clean Architecture):** разделение на слои Presentation, Application, Domain, Infrastructure.
- **SOLID:** каждый сервис и слой следует принципам SOLID для обеспечения слабой связанности и высокой модульности.
- **Dependency Injection:** все зависимости внедряются через DI-контейнер.
- **Repository Pattern:** абстракция доступа к данным через интерфейсы и репозитории.
- **Separation of Concerns:** каждый слой отвечает только за свою зону ответственности.

---

## Древовидная структура решения

```
KR2 FINAL/
├── ApiGateway/                  # ASP.NET Core шлюз
│   ├── Program.cs - Реализация через Minimal API
│   ├── appsettings.json
│   └── Dockerfile
│
├── FileStorageService/          # Сервис хранения файлов
│   ├── Presentation/            # Контроллеры
│   ├── Application/             # Бизнес-логика
│   │   ├── Services/            # FileStoringService
│   │   └── DTO/                 # FileUploadDTO
│   ├── Domain/                  # Доменная модель
│   │   ├── Entities/            # FileHolder
│   │   └── Interfaces/          # IFileStoringService, IFileStoringRepository
│   ├── Infrastructure/          # Внешние зависимости
│   │   └── Repos/               # FileStoringRepository, FileStoringDbContext
│   ├── uploads/                 # Загруженные файлы
│   ├── Migrations/              # Миграции EF Core
│   ├── Program.cs
│   └── Dockerfile
│
├── FileAnalysisService/         # Сервис анализа файлов
│   ├── Presentation/            # Контроллеры
│   ├── Application/             # Бизнес-логика
│   │   ├── Services/            # FileAnalysisService
│   ├── Domain/                  # Доменная модель
│   │   ├── Entities/            # FileDataHolder
│   │   └── Interfaces/          # IFileAnalysisService, IFileAnalysisRepository
│   ├── Infrastructure/          # Внешние зависимости
│   │   └── Repos/               # FileAnalysisRepository, FileAnalysisDbContext
│   ├── Migrations/              # Миграции EF Core
│   ├── Program.cs
│   └── Dockerfile
│
├── SharedContacts/              # Общие DTO и контракты
│   └── DTOs/                    # FileAnalysisResult
│   └── Clients/                 # Клиенты для общения сервисов между собой
│
├── FileStorageTests/            # Тесты для FileStorageService
├── FileAnalysisTests/           # Тесты для FileAnalysisService
├── docker-compose.yml           # Оркестрация сервисов
└── README.md                # Документация
```

---

## Краткое описание сервисов

### 1. **API Gateway**
- Единая точка входа для всех запросов
- Маршрутизация, балансировка, CORS, аутентификация

### 2. **File Storage Service**
- Загрузка, хранение, выдача файлов
- Метаданные в PostgreSQL, файлы в volume
- Реализация паттерна Repository

### 3. **File Analysis Service**
- Анализ текста, поиск плагиата
- Бизнес-логика анализа
- Отдельная БД PostgreSQL

---

### Доступ к сервисам
- API Gateway: http://localhost:7003
- File Storage Service: http://localhost:7001/swagger
- File Analysis Service: http://localhost:7002/swagger

---

## Документация по API

### File Storage Service API

#### Загрузка файла
```
POST /api/files
Content-Type: multipart/form-data

Response (201 Created): 
{
    "fileId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" 
}
```

#### Получение содержимого файла
```
GET /api/files/storage-request/{fileId}

Response (200 OK): 
- Текстовое содержимое файла
Content-Type: text/plain; charset=utf-8

Ошибки:  
- `404 Not Found` — если файл не существует.
```

#### Получение файла (доступно только при работа с http://localhost:7001/swagger, решил, что не стоит добавлять в возможности пользователя)
```
GET /api/files/storage/{fileId}

Response (200 OK): 
- Binary file
Content-Type: application/octet-stream

Ошибки:  
- `404 Not Found` — если файл не существует.
```
---

### File Analysis Service API

#### Получение статистики файла
```
GET /api/analysis/{fileId}
Параметры:
fileId (Guid) — уникальный идентификатор файла.

Успешный ответ (200 OK):
{
  "fileId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "hash": 12345,
  "wordCount": 150,
  "paragraphCount": 5,
  "symbolCount": 2500
}
Ошибки:
400 Bad Request — если файл не найден в хранилище или аналитической базе.
404 Not Found — если fileId некорректен.
```

#### Генерация хеша содержимого
```
GET /api/analysis/GetHash/{content}
Параметры:

content (string) — текст для анализа (передается в URL в URL-encoded формате).

Успешный ответ (200 OK):
12345
Примечание: Возвращает целое число — хеш, рассчитанный на основе содержимого.
```
    Анализ файла:
    Хеш вычисляется по формуле: (Количество слов × Количество абзацев) + Количество символов.
    Статистика сохраняется в базе данных при первом запросе. Последующие запросы возвращают кэшированные данные.
    Зависимости:
    Если файл отсутствует в аналитической базе, сервис автоматически запрашивает содержимое из FileStorageService.
---

## Инструкция по запуску

### Предварительные требования
- Docker
- Docker Compose
- .NET 8.0 SDK
- Initial миграции уже предоставлены

### Запуск системы

```bash
dotnet build
docker-compose up --build
```

### Миграции баз данных (в случае, если пользователь удалил)

```bash
# File Storage Service
cd FileStorageService
dotnet ef migrations add "InitialMigration"
dotnet ef database update (в нашем случае эта строка не нужна, всё обновляется в Program.cs)

# File Analysis Service
cd FileAnalysisService
dotnet ef migrations add "InitialMigration"
dotnet ef database update (в нашем случае эта строка не нужна, всё обновляется в Program.cs)
```

### Остановка системы
```bash
docker-compose down
```

### Очистка данных
```bash
docker-compose down -v
```

---

## Тестирование

- Юнит-тесты: xUnit + Moq
- Покрытие бизнес-логики, мокаются внешние зависимости

```bash
# File Storage Tests
cd FileStorageTests
dotnet test

# File Analysis Tests
cd FileAnalysisTests
dotnet test
```

---

## Мониторинг

Все сервисы доступны через API Gateway на порту 7003. Базы данных PostgreSQL доступны на портах 5433 (File Storage DB) и 5434 (File Analysis DB).

## Примечания
- Все сервисы работают в режиме Development
- Данные файлов хранятся в Docker volume `storage-uploads`
- Базы данных используют Docker volumes для персистентности данных
