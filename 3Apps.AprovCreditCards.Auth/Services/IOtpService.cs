namespace _3Apps.AprovCreditCards.Auth.Services;

public interface IOtpService
{
    Task<string> GenerateAsync(string email);
    Task<bool> ValidateAsync(string email, string otp);
}
