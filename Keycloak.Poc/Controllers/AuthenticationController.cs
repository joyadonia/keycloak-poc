using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Keycloak.Poc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IHttpContextAccessor httpContextAccessor) : ControllerBase
    {    
        [HttpGet]
        [Route("logout")]
        public async Task <IActionResult> Logout()
        {

            var idToken = httpContextAccessor.HttpContext.User.FindFirst("id_token")?.Value;

            if (string.IsNullOrEmpty(idToken))
            {
                return BadRequest();
            }
            var postLogoutRedirectUri = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/yams";
            var keycloakLogoutUrl = $"http://ngrp/keycloak/realms/YAMS/protocol/openid-connect/logout" +
                                    $"?id_token_hint={idToken}" +
                                    $"&post_logout_redirect_uri={postLogoutRedirectUri}";
            Console.WriteLine($"postLogoutRedirectUri {postLogoutRedirectUri}");
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = keycloakLogoutUrl
                },
                CookieAuthenticationDefaults.AuthenticationScheme
            );
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login()
        {

            var idToken = httpContextAccessor.HttpContext.User.FindFirst("id_token")?.Value;

            if (!string.IsNullOrEmpty(idToken))
            {
                return Ok();
            }

            var postLoginRedirectUri = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
           return Redirect($"http://localhost:9000/realms/YAMS/.well-known/openid-configuration"); 

         
        }
    }
}
