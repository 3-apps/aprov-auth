using Refit;

namespace _3Apps.AprovCreditCards.Auth.Services.Jarvis;

public interface IJarvisApi
{
    // Retorna HttpResponseMessage cru porque o Jarvis responde 2xx mesmo em
    // falha (template inexistente, email inválido), sinalizando o erro apenas
    // no corpo. Quem chama precisa inspecionar status HTTP E o body.
    [Post("/api/services/send-transactional")]
    Task<HttpResponseMessage> SendTransactionalEmailAsync(
        [Body] JarvisTransactionalEmailRequest body,
        [Header("x-api-key")] string apiKey,
        CancellationToken ct);
}
