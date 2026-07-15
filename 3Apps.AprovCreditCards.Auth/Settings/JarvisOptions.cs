namespace _3Apps.AprovCreditCards.Auth.Settings;

public class JarvisOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Nome da mensagem transacional cadastrada no Jarvis para o OTP.</summary>
    public string OtpTemplateName { get; set; } = "signin-otp-aprov";

    /// <summary>Chave do custom field que carrega o código do OTP (placeholder %OTPCODE% no template).</summary>
    public string OtpCodeFieldName { get; set; } = "otpCode";

    /// <summary>Chave do custom field com o tempo de expiração em minutos.</summary>
    public string OtpExpiryFieldName { get; set; } = "otpExpiryMinutes";

    /// <summary>Timeout (segundos) da chamada HTTP ao Jarvis. Endpoint é síncrono/user-facing.</summary>
    public int TimeoutSeconds { get; set; } = 10;
}
