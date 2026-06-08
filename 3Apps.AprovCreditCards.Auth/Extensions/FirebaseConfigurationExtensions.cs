using _3Apps.AprovCreditCards.Auth.Services;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace _3Apps.AprovCreditCards.Auth.Extensions;

public static class FirebaseConfigurationExtensions
{
    public static void AddFirebase(this IServiceCollection services, ConfigurationManager configuration)
    {
        var serviceAccount = configuration["Firebase:ServiceAccount"]
            ?? throw new InvalidOperationException("Firebase:ServiceAccount is not configured.");

        var credential = CredentialFactory
                  .FromJson<ServiceAccountCredential>(serviceAccount)
                  .ToGoogleCredential();

        FirebaseApp.Create(new AppOptions
        {
            Credential = credential
        });

        services.AddSingleton(FirebaseAuth.DefaultInstance);
        services.AddSingleton<IFirebaseService, FirebaseService>();

       // services.Configure<FirebaseSignInActionCodeOptions>(configuration.GetSection("Firebase:SignInActionCode"));
    }
}
