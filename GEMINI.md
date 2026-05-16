# Project: Rate (Monorepo) - Gemini CLI Context

## 🏗 Architecture Overview
Dự án là một ứng dụng Monorepo .NET hiện đại, sử dụng .NET Aspire để quản lý orchestration.

### Backend (.NET 10)
- **Kiến trúc**: Clean Architecture (Domain, Application, Infrastructure, Web).
- **Pattern**: CQRS với MediatR. Command và Handler thường được gộp chung trong một file.
- **API**: Minimal APIs trong project `Web`.
- **Orchestration**: .NET Aspire (`AppHost`) quản lý toàn bộ hệ sinh thái (Postgres, Redis, RabbitMQ, Elasticsearch, API, Frontend).

### Frontend (Blazor Web App - Interactive Auto)
- **Framework**: Blazor Web App (.NET 10).
- **Render Mode**: **Interactive Auto** (kết hợp SSR cho lần đầu và WebAssembly cho các lần sau).
- **Projects**: `WebFrontend` (Server side) và `WebFrontend.Client` (Client side/WASM).
- **Communication**: Sử dụng `IHttpClientFactory` với Service Discovery của Aspire để gọi API (`http://webapi`).

---

## 🛠 Coding Conventions

### .NET (General)
- **Naming**: PascalCase cho class/method, camelCase cho biến cục bộ. 
- **Dependencies**: Sử dụng primary constructors cho Dependency Injection.

### Backend
- **CQRS**: Các tính năng mới thêm vào `src/Application/[Feature]/Commands` hoặc `Queries`. 
- **Endpoints**: Thêm Endpoint vào `src/Web/Endpoints`.

### Blazor Frontend
- **Components**: Đặt trong `WebFrontend/Components` (SSR/Server) hoặc `WebFrontend.Client/Pages` (WASM).
- **Interactivity**: Mặc định sử dụng `@rendermode InteractiveAuto`.

---

## 🧪 Testing Guidelines
- **Unit/Integration Tests**: Nằm trong thư mục `tests/`.
- **Functional Tests**: Sử dụng `DistributedApplicationTestingBuilder` trong `tests/Application.FunctionalTests`.

---

## 🚀 Workflows cho Gemini
1. **Thêm Handler mới**: Tham khảo `src/Application/Movies/Commands/CreateMovie/CreateMovieCommand.cs`.
2. **Thêm Endpoint**: Tham khảo `src/Web/Endpoints/MovieEndpoints.cs`.
3. **Thêm Blazor Page**: Tạo `.razor` file trong `src/WebFrontend.Client/Pages`.
