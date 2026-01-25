namespace EventBus.Events;

public record IntegrationEvent
{
    [JsonInclude]
    public Guid Id { get; set; }
    [JsonInclude]
    public DateTime CreationDate { get; set; }

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
}