using ADB2C.Model.Models.OIDC;
using ADB2C.Service.OIDC.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ADB2C.Service.OIDC
{
    public class OIDCService : IOIDCService
    {
        private IConfiguration _config { get; }
        private IHttpContextAccessor _httpContext;
        private IUrlHelperFactory _urlHelperFactory;
        private IActionContextAccessor _actionContextAccessor;
        private static Lazy<X509SigningCredentials> _signingCredentials;

        public OIDCService(IConfiguration config, 
                           IHttpContextAccessor httpContext,
                           IUrlHelperFactory urlHelperFactory,
                           IActionContextAccessor actionContextAccessor)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _config = config;
            _httpContext = httpContext;
            _signingCredentials = new Lazy<X509SigningCredentials>(() =>
            {
                X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                            X509FindType.FindByThumbprint,
                                            _config["OIDC:SigningCertThumbprint"],
                                            false);

                if (certCollection.Count > 0)
                    return new X509SigningCredentials(certCollection[0]);

                throw new Exception("Certificate not found");
            });
        }

        public string BuildIdToken()
        {
            string issuer = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}{this._httpContext.HttpContext.Request.PathBase.Value}/";

            // All parameters send to Azure AD B2C needs to be sent as claims
            IList<Claim> claims = new List<Claim>();
            claims.Add(new Claim("custom_parameter", "test_value", ClaimValueTypes.String, issuer));

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer,
                    _config["AzureAdB2C:ClientId"],
                    claims,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddDays(1),
                    _signingCredentials.Value);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetJwks()
        {
            return JsonConvert.SerializeObject(new JwksModel
            {
                Keys = new[] { JwksKeyModel.FromSigningCredentials(_signingCredentials.Value) }
            });
        }

        public string GetMetadata()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            return JsonConvert.SerializeObject(new OidcModel
            {
                // Sample: The issuer name is the application root path
                Issuer = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}{_httpContext.HttpContext.Request.PathBase.Value}/",

                // Sample: Include the absolute URL to JWKs endpoint
                JwksUri = urlHelper.Link("JWKS", null),

                // Sample: Include the supported signing algorithms
                IdTokenSigningAlgValuesSupported = new[] { _signingCredentials.Value.Algorithm },
            });
        }
    }
}
