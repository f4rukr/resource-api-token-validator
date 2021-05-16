using ADB2C.Model.AuthenticationSchemes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;

namespace ADB2C.ResourceApi.Extensions.Startup
{
    public static class TokenValidationExtension
    {
        public static void UseTokenValidation(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var result = await context.AuthenticateAsync(AuthenticationSchemes.AzureADB2C);
                if (!result.Succeeded)
                    result = await context.AuthenticateAsync(AuthenticationSchemes.AzureAD);

                context.User = result?.Principal;
                await next();
            });
        }
    }
}
