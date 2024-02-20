using Microsoft.Extensions.Options;
using HubIdentificacao.src.App.Configs;

namespace HubIdentificacao.src.App.Validators
{
    public class ValidateNegotiate
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public ValidateNegotiate(RequestDelegate next, AppSettings appSettings)
        {
            _next = next;
            _appSettings = appSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            // Verifica se a solicitação é para o endpoint /identifyClient/negotiate
            if (context.Request.Path == "/identifyClient/negotiate")
            {

                var apiKey = context.Request.Headers["x-itau-apikey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    // Se os cabeçalhos não estiverem presentes, retorne uma resposta de erro
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Cabeçalhos obrigatórios não estão presentes.");
                    return;
                }
                var apiKeyDefault = _appSettings.ApiKey;
                var apiKeyIntregation = _appSettings.ApiKeyIntegration;

                if (apiKeyDefault != apiKey)
                {
                    if (apiKeyIntregation != apiKey)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("ApiKey não autorizada.");
                        return;
                    }

                }
            }
            await _next(context);
        }
    }

    public static class ValidateNegotiateExtensions
    {
        public static IApplicationBuilder UseValidateNegotiate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidateNegotiate>();
        }
    }
}