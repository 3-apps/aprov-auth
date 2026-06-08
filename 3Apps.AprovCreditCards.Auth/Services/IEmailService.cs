namespace _3Apps.AprovCreditCards.Auth.Services;

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otp, string language);
}
