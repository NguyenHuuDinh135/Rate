namespace IntegrationEventLogEF;

public class ResilientTransaction
{
    private readonly DbContext _context;

    private ResilientTransaction(DbContext context) =>
        _context = context ?? 
                   throw new ArgumentNullException(nameof(context));

    public static ResilientTransaction New(DbContext context) 
        => new(context);

    public async Task ExecuteAsync(Func<Task> action)
    {
        // Tạo execution strategy (retry khi DB fail tạm thời)
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            // Begin transaction
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            await action();

            // Commit nếu thành công
            await transaction.CommitAsync();
        });
    }
}