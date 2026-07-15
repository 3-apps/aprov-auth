using System.Text.Json.Serialization;

namespace _3Apps.AprovCreditCards.Auth.Services.Jarvis;

public class JarvisTransactionalEmailRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("contact")]
    public JarvisTransactionalContact Contact { get; set; } = default!;
}

public class JarvisTransactionalContact
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = default!;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = default!;

    [JsonPropertyName("customFields")]
    public Dictionary<string, string> CustomFields { get; set; } = new();
}
