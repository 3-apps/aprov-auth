using FirebaseAdmin.Auth;

namespace _3Apps.AprovCreditCards.Auth.Services;

public class FirebaseService(FirebaseAuth firebaseAuth) : IFirebaseService
{
    public async Task<(UserRecord, bool)> GetOrCreateUserAsync(string email)
    {
        try
        {
            var user = await firebaseAuth.GetUserByEmailAsync(email);
            return (user, false);
        }
        catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
        {
            var name = email.Contains("@") ? email.Split("@")[0] : email;
            var user = await firebaseAuth.CreateUserAsync(new UserRecordArgs { Email = email, DisplayName = name, EmailVerified = true });
            return (user, true);
        }
    }

    public Task<string> CreateCustomTokenAsync(string uid)
        => firebaseAuth.CreateCustomTokenAsync(uid);
}
