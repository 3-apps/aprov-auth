using _3Apps.AprovCreditCards.Auth.Settings;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace _3Apps.AprovCreditCards.Auth.Services;

public class EmailService(ISendGridClient sendGridClient, IOptions<SendGridOptions> settings, IOptions<OtpOptions> otpSettings) : IEmailService
{
    private readonly SendGridOptions _settings = settings.Value;
    private readonly int _expireMinutes = otpSettings.Value.ExpiryMinutes;

    public async Task SendOtpAsync(string toEmail, string otp, string language)
    {
        var message = new SendGridMessage
        {
            From = new EmailAddress(_settings.FromEmail, _settings.FromName),
            TrackingSettings = new TrackingSettings
            {
                ClickTracking = new ClickTracking { Enable = false, EnableText = false }
            },
            Asm = new ASM
            {
                GroupId = 29159
            }
        };

        message.SetTemplateId(OtpTemplateFor(language));
        message.SetTemplateData(new { OTP_CODE = otp, EXPIRY_MINUTES = _expireMinutes });
        message.AddTo(new EmailAddress(toEmail));

        var response = await sendGridClient.SendEmailAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Body.ReadAsStringAsync();
            throw new InvalidOperationException($"Falha ao enviar email. Status: {response.StatusCode}. Body: {body}");
        }
    }

    private string OtpTemplateFor(string language) => language switch
    {
        "en" => "d-4c8e065ca5f94d06ab3483eae259ad79",
        "es" => "d-4c8e065ca5f94d06ab3483eae259ad79",
        _ => "d-4c8e065ca5f94d06ab3483eae259ad79"
    };
}
