using ADB2C.Model.AuthenticationSchemes;
using ADB2C.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace ADB2C.ResourceApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.AzureADB2C)]
    public class B2CController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(
                    new User()
                    {
                        Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                        EmailAddress = User.FindFirst(ClaimTypes.Email)?.Value,
                        GivenName = User.FindFirst(ClaimTypes.GivenName)?.Value,
                        Surname = User.FindFirst(ClaimTypes.Surname)?.Value,
                    }
                );
            }

            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
