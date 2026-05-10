# 📊 Codebase Audit & Gemini Setup Report

Dự án: **Rate** (Full-stack .NET & Next.js)

## 1. Kết quả Audit Codebase

### 📱 Frontend (Next.js)
- **Kiến trúc**: Next.js App Router với Route Groups (`(app)`, `(auth)`).
- **Styling**: Tailwind CSS v4 mới nhất.
- **UI Components**: Hệ thống custom shadcn/ui (`new-york-v4`).
- **Logic**: JWT Auth với refresh token tự động, TanStack Query quản lý server state.
- **Điểm lưu ý**: Codebase rất sạch, tuân thủ chặt chẽ việc tách biệt Client/Server components.

### ⚙️ Backend (.NET 10)
- **Kiến trúc**: Clean Architecture chuẩn (Domain -> Application -> Infrastructure -> Web).
- **Patterns**: CQRS với MediatR, Minimal APIs tự động đăng ký endpoint.
- **Orchestration**: .NET Aspire quản lý hạ tầng (Postgres, Redis, RabbitMQ, Elasticsearch).
- **Messaging**: MassTransit với RabbitMQ và Outbox pattern.
- **Testing**: Functional tests tích hợp sâu với Aspire Containers.

## 2. Cấu hình môi trường Gemini

Tôi đã thiết lập các file cấu hình sau tại thư mục root của project:
- **`GEMINI.md`**: Lưu trữ toàn bộ convention, kiến trúc và workflow quan trọng để Gemini luôn nắm bắt đúng ngữ cảnh dự án.
- **`.geminiignore`**: Tối ưu hóa context window bằng cách loại bỏ các file rác, build artifacts và `node_modules`.

## 3. Custom Skills đã cài đặt

Hai kỹ năng chuyên biệt đã được tạo và cài đặt vào **Workspace Scope**:

1.  **`dotnet-cqrs-handler`**: Hỗ trợ tạo nhanh Command/Query và Handler chuẩn Clean Architecture.
2.  **`nextjs-ui-component`**: Hỗ trợ tạo UI component chuẩn Tailwind v4 và hệ thống shadcn của dự án.

> **⚠️ Lưu ý quan trọng**: Bạn cần chạy lệnh `/skills reload` trong terminal để Gemini có thể kích hoạt các kỹ năng mới này.

---
**Setup hoàn tất!** Gemini hiện đã được tối ưu hóa hoàn toàn để hỗ trợ bạn phát triển dự án Rate.
