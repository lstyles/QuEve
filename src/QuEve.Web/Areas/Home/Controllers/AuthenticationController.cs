using EVEStandard;
using EVEStandard.Models.SSO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QuEve.Core.Entities;
using QuEve.Core.Interfaces;
using QuEve.Infrastructure.Configuration;
using QuEve.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuEve.Web.Areas.Home.Controllers
{
    /// <summary>
    /// Authentication Controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Area("Home")]
    [Route("auth")]
    public class AuthenticationController : Controller
    {
        private readonly EVEStandardAPI _esiClient;
        private readonly SecurityOptions _securityOptions;

        private readonly IUniverseService _universe;

        private const string SSOStateKey = "SSOStateKey";

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="esiClient">The esi client.</param>
        /// <param name="universe">The universe.</param>
        /// <param name="securityOptions">The security options.</param>
        public AuthenticationController(
            EVEStandardAPI esiClient,
            IUniverseService universe,
            IOptions<SecurityOptions> securityOptions
            )
        {
            _esiClient = esiClient;
            _universe = universe;
            _securityOptions = securityOptions.Value;
        }

        /// <summary>
        /// Logins the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        [Route("login")]
        public IActionResult Login(string returnUrl = null)
        {
            var state = new SSOState()
            {
                StateId = Guid.NewGuid(),
                RedirectUrl = returnUrl
            };

            var stateStr = state.ToString();

            var authorization = _esiClient.SSOv2.AuthorizeToEVEUri(_securityOptions.Scopes, stateStr);
            HttpContext.Session.SetString(SSOStateKey, stateStr);
            return Redirect(authorization.SignInURI);
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        /// <returns></returns>
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Callbacks the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        [Route("callback")]
        public async Task<IActionResult> Callback(string code, string state)
        {
            var authorization = new Authorization
            {
                AuthorizationCode = code,
                ExpectedState = HttpContext.Session.GetString(SSOStateKey),
                ReturnedState = state
            };

            var stateObj = new SSOState(state);

            var accessTokenDetails = await _esiClient.SSOv2.VerifyAuthorizationAsync(authorization);
            var characterDetails = _esiClient.SSOv2.GetCharacterDetailsAsync(accessTokenDetails.AccessToken);

            var character = await _universe.GetCharacter(characterDetails.CharacterId);

            character.AccessToken = accessTokenDetails.AccessToken;
            character.AccessTokenExpiry = accessTokenDetails.ExpiresUtc;
            character.RefreshToken = accessTokenDetails.RefreshToken;

            await _universe.UpdateCharacter(character);

            await SignInAsync(character);

            if (String.IsNullOrEmpty(stateObj.RedirectUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(stateObj.RedirectUrl);
            }
        }

        /// <summary>
        /// Signs the in asynchronous.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">character</exception>
        [NonAction]
        private async Task SignInAsync(Character character)
        {
            if (character == null)
                throw new ArgumentNullException(nameof(character));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, character.CharacterId.ToString()),
                new Claim(ClaimTypes.Name, character.Name),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.UtcNow.AddHours(24) });
        }
    }
}