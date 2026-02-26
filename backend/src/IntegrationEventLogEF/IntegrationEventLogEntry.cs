using System.ComponentModel.DataAnnotations;
namespace IntegrationEventLogEF;

public class IntegrationEventLogEntry
{
    // Serialize đẹp (ghi DB)
    private static readonly JsonSerializerOptions s_indentedOptions
        = new() { WriteIndented = true };

    // Deserialize không phân biệt hoa thường
    private static readonly JsonSerializerOptions s_caseInsensitiveOptions
        = new() { PropertyNameCaseInsensitive = true };

    private IntegrationEventLogEntry() { }

    public IntegrationEventLogEntry(
        IntegrationEvent @event, 
        Guid transactionId)
    {
        EventId = @event.Id;
        CreationTime = @event.CreationDate;

        // Lưu full name để deserialize đúng type
        EventTypeName = @event.GetType().FullName;

        // Serialize event thành JSON string
        Content = JsonSerializer.Serialize(
            @event, 
            @event.GetType(), 
            s_indentedOptions);

        State = EventStateEnum.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId;
    }

    public Guid EventId { get; private set; }

    [Required]
    public string EventTypeName { get; private set; }

    // Lấy tên ngắn (không namespace)
    [NotMapped]
    public string EventTypeShortName 
        => EventTypeName.Split('.')?.Last();

    [NotMapped]
    public IntegrationEvent IntegrationEvent 
    { get; private set; }

    public EventStateEnum State { get; set; }

    public int TimesSent { get; set; }

    public DateTime CreationTime { get; private set; }

    [Required]
    public string Content { get; private set; }

    public Guid TransactionId { get; private set; }

    /// <summary>
    /// Deserialize JSON string thành object event thật
    /// </summary>
    public IntegrationEventLogEntry DeserializeJsonContent(Type type)
    {
        IntegrationEvent = JsonSerializer
                .Deserialize(Content, type, s_caseInsensitiveOptions)
            as IntegrationEvent;

        return this;
    }
}