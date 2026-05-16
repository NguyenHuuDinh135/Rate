namespace backend.Infrastructure.Redis;

internal static class RedisKeys
{
    public static string RefreshToken(string userId) => $"auth:refresh:{userId}";

    public static string RevokedToken(string jti) => $"auth:revoked:{jti}";

    public static string OneTimeToken(string purpose, string subject) => $"auth:ott:{purpose}:{subject}";

    public static string RateLimit(string key) => $"rate:{key}";

    public static string Idempotency(string key) => $"idem:{key}";
}

