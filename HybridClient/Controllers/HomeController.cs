using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using HybridClient.Models;

namespace HybridClient.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            client.SetBearerToken(accessToken);
            var response = await client.GetAsync("http://localhost:5002/identity");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await RenewTokensAsync();
                    return RedirectToAction();
                }
                throw new Exception(response.ReasonPhrase);
            }
            var content = await response.Content.ReadAsStringAsync();
            return View("Index", content);
        }

        //[Authorize(Roles = "管理员")]
        //[Authorize(Policy = "SmithInSomewhere")]
        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);//access_token
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);//id_token
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);//refresh_token
            //var code = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.Code);//code
            ViewData["accessToken"] = accessToken;
            ViewData["idToken"] = idToken;
            ViewData["refreshToken"] = refreshToken;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            //var client = new HttpClient();
            //var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            //if (disco.IsError)
            //{
            //    throw new Exception(disco.Error);
            //}

            //var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            //if (!string.IsNullOrWhiteSpace(accessToken))
            //{
            //    var revokeAccessTokenResponse = await client.RevokeTokenAsync(new TokenRevocationRequest { 
            //        Address=disco.RevocationEndpoint,
            //        ClientId="hybrid client",
            //        ClientSecret="hybrid secret",
            //        Token =accessToken
            //    });

            //    if (revokeAccessTokenResponse.IsError)
            //    {
            //        throw new Exception("Access Token Revocation Failed:"+revokeAccessTokenResponse.Error);
            //    }
            //}

            //var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            //if (!string.IsNullOrWhiteSpace(refreshToken))
            //{
            //    var revokeRefreshTokenResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
            //    {
            //        Address = disco.RevocationEndpoint,
            //        ClientId = "hybrid client",
            //        ClientSecret = "hybrid secret",
            //        Token = refreshToken
            //    });

            //    if (revokeRefreshTokenResponse.IsError)
            //    {
            //        throw new Exception("Refresh Token Revocation Failed:" + revokeRefreshTokenResponse.Error);
            //    }
            //}
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        private async Task<string> RenewTokensAsync()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // Refresh Access Token
            var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "mvc client",
                ClientSecret = "mvc client",
                Scope = "api1 openid profile email phone address",
                GrantType = OpenIdConnectGrantTypes.RefreshToken,
                RefreshToken = refreshToken
            });

            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
            var tokens = new[]
            {
                new AuthenticationToken
                {
                    Name=OpenIdConnectParameterNames.IdToken,
                    Value=tokenResponse.IdentityToken,
                },
                new AuthenticationToken
                {
                    Name=OpenIdConnectParameterNames.AccessToken,
                    Value=tokenResponse.AccessToken,
                },
                new AuthenticationToken
                {
                    Name=OpenIdConnectParameterNames.RefreshToken,
                    Value=tokenResponse.RefreshToken,
                },
                new AuthenticationToken
                {
                    Name="expires_at",
                    Value=expiresAt.ToString("o",CultureInfo.InvariantCulture)
                }

            };
            // 获身份认证结果，包含当前的principal和properties
            var currentAuthenticateResult =
            await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //把新的tokens存储起来
            currentAuthenticateResult.Properties.StoreTokens(tokens);

            //登陆
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);

            return tokenResponse.AccessToken;
        }
    }
}
