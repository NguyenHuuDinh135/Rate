# Rate - Modern Full-stack Monorepo (.NET 10 & Aspire)

Rate là một hệ sinh thái ứng dụng hiện đại, được xây dựng theo kiến trúc Monorepo tối ưu, cho phép chia sẻ mã nguồn tối đa giữa Web, Mobile và Backend. Dự án sử dụng những công nghệ tiên tiến nhất trong hệ sinh thái Microsoft năm 2026.

## 🚀 Technology Stack

### Backend
- **Runtime**: .NET 10 (Preview)
- **Architecture**: Clean Architecture + CQRS với MediatR.
- **Orchestration**: .NET Aspire (Quản lý Postgres, Redis, RabbitMQ, Elasticsearch).
- **API**: Minimal APIs (Type-safe, High performance).
- **Persistence**: EF Core + PostgreSQL + Dapper.

### Frontend (Web & Mobile)
- **Shared UI**: Razor Class Library (`WebFrontend.Shared`) chứa 100% components dùng chung.
- **Web**: Blazor Web App (Interactive Auto mode - SSR + WASM).
- **Mobile**: .NET MAUI Blazor Hybrid (iOS, Android, Windows, macOS).
- **UI Component**: Microsoft Fluent UI Blazor v4.
- **Styling**: Tailwind CSS v4.
- **State Management**: Fluxor (Redux pattern).
- **API Client**: Refit (Type-safe REST library).

## 🏗 Project Structure

```text
Rate/
├── src/
│   ├── AppHost/                # .NET Aspire Orchestrator
│   ├── ServiceDefaults/         # Cấu hình chung cho Resilience, Telemetry
│   ├── Domain/                 # Enterprise logic (Entities, Value Objects)
│   ├── Application/            # Business logic (CQRS Handlers, Validators)
│   ├── Infrastructure/         # External concerns (DB, Identity, File System)
│   ├── Web/                    # Minimal API Project
│   ├── WebFrontend/            # Blazor Server Host
│   ├── WebFrontend.Client/     # Blazor WebAssembly Client
│   ├── WebFrontend.Shared/     # Shared Razor Components & Logic (Web & Mobile)
│   └── MobileApp/              # .NET MAUI Mobile App Container
├── tests/                      # Unit, Integration & Functional Tests
└── Rate.slnx                   # Modern Visual Studio Solution file
```

## 🛠 Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (cho Aspire resources)
- [Visual Studio 2022 (v17.13+)](https://visualstudio.microsoft.com/) hoặc VS Code với C# Dev Kit.

## 🏃 Getting Started

1. **Clone the repository**:
   ```bash
   git clone https://github.com/NguyenHuuDinh135/Rate.git
   cd Rate
   ```

2. **Run with .NET Aspire**:
   Mở `Rate.slnx` và chạy project `AppHost`, hoặc dùng CLI:
   ```bash
   dotnet run --project src/AppHost/AppHost.csproj
   ```

3. **Develop UI**:
   Mọi thay đổi giao diện nên thực hiện trong `src/WebFrontend.Shared`. Tailwind v4 sẽ tự động biên dịch CSS khi bạn build project Web hoặc Mobile.

## 🤖 AI-Powered Development
Dự án này được tối ưu hóa cho phát triển bằng AI agent (như Gemini CLI). 
- Sử dụng skill `dotnet-clean-architecture-jasontaylor` để scaffold tính năng mới.
- Hệ thống sẵn sàng với các bộ khung validator và DTO dùng chung giúp AI hiểu ngữ cảnh dự án nhanh nhất.

---
Built with ❤️ by [NguyenHuuDinh135](https://github.com/NguyenHuuDinh135)
