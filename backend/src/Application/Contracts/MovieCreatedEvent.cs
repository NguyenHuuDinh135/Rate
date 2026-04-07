namespace backend.Application.Common.Contracts;

public record MovieCreatedEvent
{
    // Thêm constructor mặc định cho MassTransit
    public MovieCreatedEvent() { }

    public MovieCreatedEvent(int id, string title)
    {
        Id = id;
        Title = title;
    }

    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
}