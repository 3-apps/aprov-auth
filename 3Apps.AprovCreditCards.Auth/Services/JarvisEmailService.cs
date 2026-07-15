using System.Text.Json;
using _3Apps.AprovCreditCards.Auth.Services.Jarvis;
using _3Apps.AprovCreditCards.Auth.Settings;
using Microsoft.Extensions.Options;

namespace _3Apps.AprovCreditCards.Auth.Services;

public class JarvisEmailService(
    IJarvisApi jarvisApi,
    IOptions<JarvisOptions> settings,
    IOptions<OtpOptions> otpSettings) : IEmailService
{
    private readonly JarvisOptions _settings = settings.Value;
    private readonly int _expireMinutes = otpSettings.Value.ExpiryMinutes;

    public async Task SendOtpAsync(string toEmail, string otp, string language)
    {
        // O Jarvis usa um único template para o OTP; 'language' é mantido na
        // assinatura por compatibilidade, mas não altera o template hoje.
        // O contato vai sem nome de propósito: no /otp/request só temos o email,
        // e o send-transactional NÃO faz upsert de contato (não polui o CRM).
        var payload = new JarvisTransactionalEmailRequest
        {
            Name = _settings.OtpTemplateName,
            Contact = new JarvisTransactionalContact
            {
                Email = toEmail,
                FirstName = string.Empty,
                LastName = string.Empty,
                CustomFields = new Dictionary<string, string>
                {
                    [_settings.OtpCodeFieldName] = otp,
                    [_settings.OtpExpiryFieldName] = _expireMinutes.ToString()
                }
            }
        };

        using var response = await jarvisApi.SendTransactionalEmailAsync(payload, _settings.ApiKey, CancellationToken.None);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Falha ao enviar email de OTP via Jarvis. Status: {(int)response.StatusCode}. Body: {body}");

        // Jarvis responde 2xx mesmo em falha; só é sucesso quando o corpo traz status == "ok".
        if (!IsQueued(body))
            throw new InvalidOperationException(
                $"Jarvis não enfileirou o email de OTP. Resposta: {body}");
    }

    private static bool IsQueued(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.TryGetProperty("status", out var status)
                   && status.ValueKind == JsonValueKind.String
                   && string.Equals(status.GetString(), "ok", StringComparison.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
