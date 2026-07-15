using _3Apps.AprovCreditCards.Auth.Services;
using _3Apps.AprovCreditCards.Auth.Services.Jarvis;
using _3Apps.AprovCreditCards.Auth.Settings;
using Refit;

namespace _3Apps.AprovCreditCards.Auth.Extensions;

public static class JarvisConfigurationExtensions
{
    public static void AddJarvis(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<JarvisOptions>(configuration.GetSection("Jarvis"));

        var baseUrl = configuration["Jarvis:BaseUrl"]
            ?? throw new InvalidOperationException("Jarvis:BaseUrl is not configured.");

        // Falha rápido no startup se a API key não estiver no Key Vault/config,
        // em vez de descobrir só quando o primeiro OTP falhar em runtime.
        _ = configuration["Jarvis:ApiKey"]
            ?? throw new InvalidOperationException("Jarvis:ApiKey is not configured.");

        var timeoutSeconds = int.TryParse(configuration["Jarvis:TimeoutSeconds"], out var t) ? t : 10;

        services
            .AddRefitClient<IJarvisApi>(new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer()
            })
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            });

        services.AddScoped<IEmailService, JarvisEmailService>();
    }
}
