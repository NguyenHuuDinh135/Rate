using System.ComponentModel.DataAnnotations;

namespace backend.Infrastructure.Caching;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";

    public string InstanceName { get; init; } = "backend";

    [Range(1, int.MaxValue)]
    public int DefaultTtlSeconds { get; init; } = 300;
}

