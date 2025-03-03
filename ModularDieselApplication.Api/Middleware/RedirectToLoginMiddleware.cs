﻿namespace ModularDieselApplication.Application.MIddleware
{
    public class RedirectToLoginMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

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
