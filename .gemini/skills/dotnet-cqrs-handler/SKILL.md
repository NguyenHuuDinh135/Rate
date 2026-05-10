---
name: dotnet-cqrs-handler
description: Scaffold Command/Query and Handler for .NET Clean Architecture using MediatR. Use this when adding new features or logic to the backend/src/Application directory.
---

# .NET CQRS Handler

Skill này giúp tạo nhanh cấu trúc Command/Query và Handler theo đúng pattern của dự án Rate.

## Workflow

1. **Xác định Feature**: Feature nằm trong thư mục `backend/src/Application/[Feature]`.
2. **Cấu trúc File**: Gộp cả Command/Query Record và Handler Class vào cùng một file.
3. **Namespace**: Tuân thủ `backend.Application.[Feature].[Commands|Queries].[Name]`.
4. **Primary Constructor**: Sử dụng primary constructor cho Handler để inject dependencies (VD: `IApplicationDbContext`, `ICacheService`).
5. **Result Pattern**: Luôn trả về `Result<T>` hoặc `Result<int>`.

## Template Ví dụ

```csharp
namespace backend.Application.Movies.Commands.CreateMovie;

public sealed record CreateMovieCommand : IRequest<Result<int>>
{
    // Properties
}

public sealed class CreateMovieCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateMovieCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateMovieCommand request, CancellationToken ct)
    {
        // Logic
        return Result<int>.Success(id);
    }
}
```
