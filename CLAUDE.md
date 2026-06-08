# aprov-auth

API de autenticação via OTP por email para o produto Aprov Credit Cards.

## Stack

- .NET 10 / ASP.NET Core
- Redis (IDistributedCache via StackExchange.Redis)
- Firebase Admin SDK (autenticação de usuários, custom tokens)
- SendGrid (envio de email com template dinâmico)
- Azure Key Vault (secrets em produção via DefaultAzureCredential)
- Application Insights (telemetria)

## Estrutura

```
3Apps.AprovCreditCards.Auth/
├── Controllers/        # AuthController
├── Exceptions/         # OtpCooldownException
├── Extensions/         # IServiceCollection extensions (Firebase, SendGrid, Redis, KeyVault)
├── Infrastructure/     # GlobalExceptionHandler (IExceptionHandler)
├── Services/           # OtpService, EmailService, FirebaseService + interfaces
└── Settings/           # OtpOptions, SendGridOptions
```

## Configuração necessária

As seguintes chaves devem estar presentes (via Key Vault em produção, appsettings.*.json em dev):

| Chave | Descrição |
|---|---|
| `KeyVaultConfig:Uri` | URI do Azure Key Vault |
| `Redis:ConnectionString` | Connection string do Redis (ex: `localhost:6379`) |
| `SendGrid:ApiKey` | API key do SendGrid |
| `SendGrid:FromEmail` | Email remetente |
| `SendGrid:FromName` | Nome remetente (padrão: "Aprov") |
| `Firebase:ServiceAccount` | JSON da service account do Firebase (string) |
| `ApplicationInsights:ApiAuthConnectionString` | Connection string do App Insights |

Valores em `appsettings.json`:
```json
{
  "Otp": {
    "ExpiryMinutes": 10,
    "MaxAttempts": 5,
    "Length": 5
  }
}
```

## Fluxo de autenticação

1. `POST /auth/otp/request` — gera OTP e envia por email
2. `POST /auth/otp/verify` — valida OTP e retorna Firebase custom token

## Comportamento do OTP

- Código numérico com `Length` dígitos (padrão: 5)
- Expira em `ExpiryMinutes` minutos (padrão: 10)
- Máximo de `MaxAttempts` tentativas erradas (padrão: 5)
- Cooldown de 2 minutos entre reenvios
- Estado armazenado em uma única chave Redis `otp:{email}` como JSON (`OtpCacheEntry`)

## Erros

Todos os erros retornam Problem Details (RFC 7807). Exceções não tratadas são capturadas pelo `GlobalExceptionHandler` e retornam 500.
