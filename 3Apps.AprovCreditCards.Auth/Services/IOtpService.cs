namespace _3Apps.AprovCreditCards.Auth.Services;

public interface IOtpService
{
    Task<string> GenerateAsync(string email);
    Task<bool> ValidateAsync(string email, string otp);

    /// <summary>
    /// Remove o OTP (e o cooldown) do cache. Usado quando o envio do email falha,
    /// para não deixar o usuário travado sem ter recebido o código.
    /// </summary>
    Task InvalidateAsync(string email);
}
