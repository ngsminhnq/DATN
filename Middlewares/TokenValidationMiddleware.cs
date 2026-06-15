using Microsoft.AspNetCore.Authorization;

namespace HRemployee.Middlewares
{

    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenValidationMiddleware> _logger;

        public TokenValidationMiddleware(RequestDelegate next,
            ILogger<TokenValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var endpoint = context.GetEndpoint();
            var authAttribute = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>();

            if (authAttribute == null)
            {
                await _next(context);
                return;
            }

            if (!TryExtractToken(context, out _))
                return;

            await _next(context);
        }

        private static bool TryExtractToken(HttpContext context, out string token)
        {
            token = null;
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.WriteAsJsonAsync(new
                {
                    status = 401, message = authHeader == null ? "Bạn cần đăng nhập để truy cập API này" : "Định dạng header Authorization không hợp lệ. Dùng: Bearer {token}" }).Wait();
                return false;
            }

            token = authHeader.Substring("Bearer ".Length).Trim();
            return true;
        }
    }
}