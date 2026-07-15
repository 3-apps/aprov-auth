using System.Text.Json;
using _3Apps.AprovCreditCards.Auth.Exceptions;
using _3Apps.AprovCreditCards.Auth.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace _3Apps.AprovCreditCards.Auth.Services;

record OtpCacheEntry(string Code, int Attempts, DateTimeOffset SentAt);

public class OtpService(IDistributedCache cache, IOptions<OtpOptions> options) : IOtpService
{
    private readonly OtpOptions _options = options.Value;

    private static readonly TimeSpan CooldownDuration = TimeSpan.FromMinutes(2);

    private static string CacheKey(string email) => $"otp:{email.ToLowerInvariant()}";

    public async Task<string> GenerateAsync(string email)
    {
        var existing = await GetEntryAsync(email);
        if (existing is not null)
        {
            var remaining = CooldownDuration - (DateTimeOffset.UtcNow - existing.SentAt);
            if (remaining > TimeSpan.Zero)
                throw new OtpCooldownException(remaining);
        }

        var otp = Random.Shared.Next(0, (int)Math.Pow(10, _options.Length))
            .ToString()
            .PadLeft(_options.Length, '0');

        await SetEntryAsync(email, new OtpCacheEntry(otp, 0, DateTimeOffset.UtcNow));

        return otp;
    }

    public async Task<bool> ValidateAsync(string email, string otp)
    {
        var entry = await GetEntryAsync(email);
        if (entry is null)
            return false;

        if (entry.Attempts >= _options.MaxAttempts)
            return false;

        if (entry.Code != otp)
        {
            await SetEntryAsync(email, entry with { Attempts = entry.Attempts + 1 });
            return false;
        }

        await cache.RemoveAsync(CacheKey(email));
        return true;
    }

    public Task InvalidateAsync(string email) => cache.RemoveAsync(CacheKey(email));

    private async Task<OtpCacheEntry?> GetEntryAsync(string email)
    {
        var json = await cache.GetStringAsync(CacheKey(email));
        return json is null ? null : JsonSerializer.Deserialize<OtpCacheEntry>(json);
    }

    private Task SetEntryAsync(string email, OtpCacheEntry entry)
    {
        var expiry = entry.SentAt.AddMinutes(_options.ExpiryMinutes) - DateTimeOffset.UtcNow;
        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry > TimeSpan.Zero ? expiry : TimeSpan.FromSeconds(1)
        };

        return cache.SetStringAsync(CacheKey(email), JsonSerializer.Serialize(entry), entryOptions);
    }
}
