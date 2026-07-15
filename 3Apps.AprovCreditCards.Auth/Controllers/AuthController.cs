using _3Apps.AprovCreditCards.Auth.Exceptions;
using _3Apps.AprovCreditCards.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace _3Apps.AprovCreditCards.Auth.Controllers;

public record RequestOtpRequest(string Email, string Language = "pt");
public record VerifyOtpRequest(string Email, string Otp);
public record VerifyOtpResponse(string CustomToken, bool IsNewUser);

[ApiController]
[Route("auth")]
public class AuthController(
    IOtpService otpService,
    IEmailService emailService,
    IFirebaseService firebaseService,
    ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("otp/request")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpRequest request)
    {
        string otp;
        try
        {
            otp = await otpService.GenerateAsync(request.Email);
        }
        catch (OtpCooldownException ex)
        {
            return Problem(
                title: "Too many requests.",
                detail: ex.Message,
                statusCode: StatusCodes.Status429TooManyRequests);
        }

        try
        {
            await emailService.SendOtpAsync(request.Email, otp, request.Language);
        }
        catch (Exception ex)
        {
            // O OTP já foi gravado (com cooldown) antes do envio. Se o envio falhar,
            // invalidamos o registro para o usuário não ficar travado sem o código.
            await otpService.InvalidateAsync(request.Email);
            logger.LogError(ex, "Falha ao enviar OTP para {Email}", request.Email);
            return Problem(
                title: "Falha ao enviar o email de OTP.",
                detail: "Não foi possível enviar o código no momento. Tente novamente.",
                statusCode: StatusCodes.Status502BadGateway);
        }

        return Ok();
    }

    [HttpPost("otp/verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        if (!await otpService.ValidateAsync(request.Email, request.Otp))
            return Problem(
                title: "OTP inválido ou expirado.",
                statusCode: StatusCodes.Status401Unauthorized);

        var (user, isNewUser) = await firebaseService.GetOrCreateUserAsync(request.Email);
        var customToken = await firebaseService.CreateCustomTokenAsync(user.Uid);

        return Ok(new VerifyOtpResponse(customToken, isNewUser));
    }
}
