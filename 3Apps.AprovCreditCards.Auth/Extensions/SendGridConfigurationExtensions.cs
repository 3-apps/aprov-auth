using _3Apps.AprovCreditCards.Auth.Services;
using _3Apps.AprovCreditCards.Auth.Settings;
using SendGrid;

namespace _3Apps.AprovCreditCards.Auth.Extensions;

public static class SendGridConfigurationExtensions
{
    public static void AddSendGrid(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<SendGridOptions>(configuration.GetSection("SendGrid"));

        var sendGridApiKey = configuration["SendGrid:ApiKey"]
            ?? throw new InvalidOperationException("SendGrid:ApiKey is not configured.");

        services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));
        services.AddScoped<IEmailService, EmailService>();
    }
}
