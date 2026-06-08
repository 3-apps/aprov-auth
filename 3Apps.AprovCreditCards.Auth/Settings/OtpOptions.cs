namespace _3Apps.AprovCreditCards.Auth.Settings;

public class OtpOptions
{
    public int ExpiryMinutes { get; set; }
    public int MaxAttempts { get; set; }
    public int Length { get; set; } = 5;
}
