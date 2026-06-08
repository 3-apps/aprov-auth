# API Docs — aprov-auth

Base URL: `/auth`

Todos os erros seguem o formato [Problem Details (RFC 7807)](https://datatracker.ietf.org/doc/html/rfc7807).

---

## POST /auth/otp/request

Gera um OTP e envia por email para o usuário.

### Request

```json
{
  "email": "usuario@exemplo.com",
  "language": "pt"
}
```

| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `email` | string | sim | Email do destinatário |
| `language` | string | não | Idioma do email: `pt` (padrão), `en`, `es` |

### Responses

**200 OK** — OTP enviado com sucesso. Corpo vazio.

**429 Too Many Requests** — Um OTP já foi enviado nos últimos 2 minutos.

```json
{
  "title": "Too many requests.",
  "detail": "An OTP was already sent. Please wait 87 seconds before requesting a new one.",
  "status": 429
}
```

**500 Internal Server Error** — Falha inesperada (ex: Redis indisponível, erro no SendGrid).

```json
{
  "title": "Internal Server Error",
  "detail": "An unexpected error occurred. Please try again later.",
  "status": 500
}
```

---

## POST /auth/otp/verify

Valida o OTP informado e retorna um Firebase custom token.

### Request

```json
{
  "email": "usuario@exemplo.com",
  "otp": "48291"
}
```

| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `email` | string | sim | Email do usuário |
| `otp` | string | sim | Código OTP recebido por email |

### Responses

**200 OK** — OTP válido. Retorna o custom token do Firebase.

```json
{
  "customToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "isNewUser": false
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `customToken` | string | Firebase custom token para autenticação no cliente |
| `isNewUser` | bool | `true` se o usuário foi criado agora no Firebase |

**401 Unauthorized** — OTP inválido, expirado ou tentativas excedidas.

```json
{
  "title": "OTP inválido ou expirado.",
  "status": 401
}
```

**500 Internal Server Error** — Falha inesperada.

```json
{
  "title": "Internal Server Error",
  "detail": "An unexpected error occurred. Please try again later.",
  "status": 500
}
```

---

## Regras de negócio

- O OTP expira em 10 minutos após o envio.
- Após 5 tentativas erradas, o OTP é invalidado (retorna 401).
- Novo OTP só pode ser solicitado após 2 minutos do envio anterior (cooldown).
- Após validação bem-sucedida, o OTP é removido do cache imediatamente.
