namespace ModularDieselApplication.Api.Middleware
{
    public class RedirectToLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectToLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // ----------------------------------------
        // Redirect unauthenticated users to the login page.
        // ----------------------------------------
        public async Task InvokeAsync(HttpContext context)
        {
            bool isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;
            bool isLoginPath = context.Request.Path.StartsWithSegments("/Login", StringComparison.OrdinalIgnoreCase);

            if (!isAuthenticated && !isLoginPath)
            {
                context.Response.Redirect("/Login/Index");
                return;
            }

            await _next(context);
        }
    }
}
