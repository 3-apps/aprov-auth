namespace _3Apps.AprovCreditCards.Auth.Exceptions;

public class OtpCooldownException(TimeSpan remaining)
    : Exception($"An OTP was already sent. Please wait {(int)remaining.TotalSeconds} seconds before requesting a new one.")
{
    public TimeSpan Remaining { get; } = remaining;
}
