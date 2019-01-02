using EVEStandard;
using EVEStandard.Enumerations;
using EVEStandard.Models.SSO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            var callbackUrl = "https://localhost:44328/auth/callback";
            var clientId = "";
            var scopes = new List<string>()
            {
                "publicData"
            };

            var esiClient = new EVEStandardAPI(
                    "QuEve",
                    DataSource.Tranquility,
                    TimeSpan.FromSeconds(30),
                    callbackUrl,
                    clientId,
                    null, // Don't need a secret for non-web app
                    SSOVersion.v2,
                    SSOMode.Native
                );

            var state = Guid.NewGuid().ToString();
            var authorization = esiClient.SSOv2.AuthorizeToEVEUri(scopes, state);

            // https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
            var url = authorization.SignInURI.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });

            Console.WriteLine("Paste callback url from the browser:");
            var returnUrl = new Uri(Console.ReadLine());
            var queryString = returnUrl.Query;
            var queryDict = HttpUtility.ParseQueryString(queryString);

            var code = queryDict["code"];
            var state2 = queryDict["state"];

            authorization.AuthorizationCode = code;
            authorization.ExpectedState = state;
            authorization.ReturnedState = state2;

            var accessTokenDetails = await esiClient.SSOv2.VerifyAuthorizationAsync(authorization);
            var characterDetails = esiClient.SSOv2.GetCharacterDetailsAsync(accessTokenDetails.AccessToken);

            var refreshedAccessToken = await esiClient.SSOv2.GetRefreshTokenAsync(accessTokenDetails.RefreshToken);

            await esiClient.SSOv2.RevokeTokenAsync(RevokeType.REFRESH_TOKEN, refreshedAccessToken.RefreshToken);
            await esiClient.SSOv2.RevokeTokenAsync(RevokeType.ACCESS_TOKEN, refreshedAccessToken.AccessToken);
        }
    }
}
