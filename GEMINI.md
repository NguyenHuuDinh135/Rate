# Project: Rate (Full-stack) - Gemini CLI Context

## 🏗 Architecture Overview
Dự án là một ứng dụng Full-stack hiện đại, được xây dựng theo kiến trúc phân lớp và microservices-ready.

### Backend (.NET 10 & Aspire)
- **Kiến trúc**: Clean Architecture (Domain, Application, Infrastructure, Web).
- **Pattern**: CQRS với MediatR. Command và Handler thường được gộp chung trong một file.
- **API**: Minimal APIs với cơ chế tự động đăng ký endpoint (`IEndpointGroup`).
- **Persistence**: EF Core với PostgreSQL (Npgsql). Sử dụng Auditable entities và Domain Events.
- **Orchestration**: .NET Aspire (`AppHost`) quản lý Postgres, Redis, RabbitMQ, và Elasticsearch.
- **Messaging**: MassTransit + RabbitMQ + EF Outbox.
- **Jobs**: Hangfire (Postgres storage).

### Frontend (Next.js & Tailwind v4)
- **Framework**: Next.js App Router (với Route Groups `(app)` và `(auth)`).
- **Styling**: Tailwind CSS v4 (sử dụng `@theme` block và CSS variables).
- **UI System**: Custom shadcn/ui registry (`registry/new-york-v4`).
- **State Management**: TanStack Query (Server state) và React Context (Auth state).
- **Form/Validation**: React Hook Form + Zod.
- **API Client**: Fetch-based `apiClient` với cơ chế tự động refresh token (interceptors).

---

## 🛠 Coding Conventions

### Backend (.NET)
- **CQRS**: Các tính năng mới nên được thêm vào `Application/[Feature]/Commands` hoặc `Application/[Feature]/Queries`. 
- **Surgical Changes**: Khi sửa logic, hãy tập trung vào Handler. Khi sửa API, hãy kiểm tra `Web/Endpoints`.
- **Naming**: Sử dụng PascalCase cho class/method, camelCase cho biến cục bộ. 
- **Dependencies**: Sử dụng primary constructors cho Dependency Injection.

### Frontend (Next.js)
- **Components**: Ưu tiên Server Components. Chỉ sử dụng `'use client'` khi cần interactivity hoặc hooks.
- **Styles**: Sử dụng utility classes của Tailwind CSS v4. Tránh viết CSS thuần trừ khi thực sự cần thiết.
- **API**: Luôn sử dụng `apiClient` từ `lib/api/api-client.ts` để đảm bảo auth/refresh logic.
- **Validation**: Định nghĩa Zod schema trong `lib/validations`.

---

## 🧪 Testing Guidelines
- **Backend**: Sử dụng NUnit và Shouldly. Functional tests nằm trong `tests/Application.FunctionalTests`, sử dụng Aspire `DistributedApplicationTestingBuilder`.
- **Frontend**: Hiện chưa thấy framework test rõ rệt, tuân thủ cấu trúc component hiện tại.

---

## 🚀 Workflows cho Gemini
1. **Thêm Handler mới**: Tham khảo `backend/src/Application/Movies/Commands/CreateMovie/CreateMovieCommand.cs`.
2. **Thêm UI Component**: Tham khảo `frontend/registry/new-york-v4/ui/`.
3. **Thêm Endpoint**: Tạo class implement `IEndpointGroup` trong `backend/src/Web/Endpoints`.
