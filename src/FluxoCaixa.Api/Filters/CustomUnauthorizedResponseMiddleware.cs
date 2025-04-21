using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace FluxoCaixa.Api.Filters
{
    public class CustomUnauthorizedResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomUnauthorizedResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = 401,
                    message = "Você precisa estar autenticado para acessar este recurso. Por favor, faça login e envie um token JWT válido."
                });
                await context.Response.WriteAsync(result);
            }
        }
    }
}
