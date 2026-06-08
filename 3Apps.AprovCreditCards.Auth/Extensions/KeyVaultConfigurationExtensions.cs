using Azure.Identity;

namespace _3Apps.AprovCreditCards.Auth.Extensions;

public static class KeyVaultConfigurationExtensions
{
    public static void AddKeyVault(this IServiceCollection services, ConfigurationManager configuration)
    {
        var keyVaultUri = new Uri(configuration["KeyVaultConfig:Uri"]!);
        configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
    }
}
