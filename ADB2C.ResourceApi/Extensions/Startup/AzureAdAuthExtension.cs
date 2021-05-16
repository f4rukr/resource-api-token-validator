using ADB2C.Model.AuthenticationSchemes;
using ADB2C.Model.Constants;
using ADB2C.Model.Models.OIDC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ADB2C.ResourceApi.Extensions.Startup
{
    public static class AzureAdAuthExtension
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication()
              .AddJwtBearer(AuthenticationSchemes.AzureADB2C, options =>
            {
                options.Authority = $"{Configuration["AzureAdB2C:Instance"]}/tfp/{Configuration["AzureAdB2C:Tenant"]}/{Configuration["AzureAdB2C:PolicyId"]}/v2.0/";
                options.Audience = Configuration["AzureAdB2C:ResourceId"].Replace("/.default", string.Empty);
            }).AddJwtBearer(AuthenticationSchemes.AzureAD, options =>
            {
                options.Authority = $"{Configuration["AzureAd:Instance"]}/{Configuration["AzureAd:TenantId"]}";
                options.Audience = Configuration["AzureAd:Audience"].Replace("/.default", string.Empty);

                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = async context => //event handler on auth errors
                    {
                        context.NoResult();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var res = new AuthErrorResponse
                        {
                            Code = "error_code",
                            UserMessage = AuthorizationErrorMessages.TokenInvalid.UserMessage,
                            DeveloperMessage = AuthorizationErrorMessages.TokenInvalid.DeveloperMessage,
                            Status = StatusCodes.Status401Unauthorized,
                            RequestId = context.HttpContext.Connection.Id,
                            Version = ApiVersion.Default.MajorVersion.ToString()
                        };

                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                            res.UserMessage = AuthorizationErrorMessages.TokenExpired.UserMessage;
                            res.DeveloperMessage = AuthorizationErrorMessages.TokenExpired.DeveloperMessage;
                        }

                        var resJson = JsonConvert.SerializeObject(res);
                        await context.Response.WriteAsync(resJson);
                    },
                    OnChallenge = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var res = new AuthErrorResponse
                        {
                            Code = "error_code",
                            UserMessage = AuthorizationErrorMessages.TokenNotProvided.UserMessage,
                            DeveloperMessage = AuthorizationErrorMessages.TokenNotProvided.DeveloperMessage,
                            Status = StatusCodes.Status401Unauthorized,
                            RequestId = context.HttpContext.Connection.Id,
                            Version = ApiVersion.Default.MajorVersion.ToString()
                        };

                        var resJson = JsonConvert.SerializeObject(res);
                        await context.Response.WriteAsync(resJson);
                    }
                };
            });
        }
    }
}
