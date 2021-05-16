using ADB2C.Model.AuthenticationSchemes;
using ADB2C.Model.RoleTypes;
using ADB2C.Service.OIDC.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ADB2C.ResourceApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OidcController : ControllerBase
    {
        private IOIDCService _oidcService;
        public OidcController(IOIDCService oidcService)
        {
            _oidcService = oidcService;
        }

        [HttpGet("idtoken", Name = "IdToken")]
        [Authorize(AuthenticationSchemes = AuthenticationSchemes.AzureAD, Roles = RoleTypeClaims.TokenHint)]
        public ActionResult IdToken()
        {
            try
            {
                return Ok(_oidcService.BuildIdToken());
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet(".well-known/openid-configuration", Name = "OIDCMetadata")]
        public ActionResult Metadata()
        {
            try
            {
                return Content(_oidcService.GetMetadata(), "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet(".well-known/keys", Name = "JWKS")]
        public ActionResult JwksDocument()
        {
            try
            {
                return Content(_oidcService.GetJwks(), "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
