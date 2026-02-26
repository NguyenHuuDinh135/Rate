namespace IntegrationEventLogEF;

public enum EventStateEnum
{
    NotPublished = 0,      // chưa publish
    InProgress = 1,        // đang publish
    Published = 2,         // publish thành công
    PublishedFailed = 3    // publish thất bại
}