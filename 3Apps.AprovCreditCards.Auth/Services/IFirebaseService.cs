using FirebaseAdmin.Auth;

namespace _3Apps.AprovCreditCards.Auth.Services;

public interface IFirebaseService
{
    Task<(UserRecord, bool)> GetOrCreateUserAsync(string email);
    Task<string> CreateCustomTokenAsync(string uid);
}
