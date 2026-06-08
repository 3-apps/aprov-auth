using _3Apps.AprovCreditCards.Auth.Extensions;
using _3Apps.AprovCreditCards.Auth.Infrastructure;
using _3Apps.AprovCreditCards.Auth.Services;
using _3Apps.AprovCreditCards.Auth.Settings;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKeyVault(builder.Configuration);
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ApiAuthConnectionString"];
});
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddFirebase(builder.Configuration);
builder.Services.AddSendGrid(builder.Configuration);
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.Configure<OtpOptions>(builder.Configuration.GetSection("Otp"));
builder.Services.AddSingleton<IOtpService, OtpService>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
