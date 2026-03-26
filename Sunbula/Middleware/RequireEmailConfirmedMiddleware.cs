using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Sunbula.Middleware
{
    public class RequireEmailConfirmedMiddleware
    {
        private readonly RequestDelegate _next;

        public RequireEmailConfirmedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply if user is authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Let authentication, email confirm endpoints pass through
                if (!context.Request.Path.StartsWithSegments("/api/v1/authentication", StringComparison.OrdinalIgnoreCase) &&
                    !context.Request.Path.StartsWithSegments("/api/authentication", StringComparison.OrdinalIgnoreCase))
                {
                    var isEmailConfirmedClaim = context.User.FindFirst("IsEmailConfirmed")?.Value;
                    if (!bool.TryParse(isEmailConfirmedClaim, out var isConfirmed) || !isConfirmed)
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"error\": \"Email verification is required to access this endpoint.\"}");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
