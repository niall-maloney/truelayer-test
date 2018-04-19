using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace TrueLayerTest
{
    public class TrueLayerAuthHandler : OAuthHandler<TrueLayerOptions>
    {
        public TrueLayerAuthHandler(IOptionsMonitor<TrueLayerOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            var scope = FormatScope();

            var state = Options.StateDataFormat.Protect(properties);
            var parameters = new Dictionary<string, string>
            {
                {"client_id", Options.ClientId},
                {"scope", scope},
                {"response_type", "code"},
                {"redirect_uri", redirectUri},
                {"state", state},
                {"enable_mock", Options.EnableMock.ToString().ToLower()}
            };

            return QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, parameters);
        }

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            var tokenRequestParameters = new Dictionary<string, string>
            {
                {"client_id", Options.ClientId},
                {"redirect_uri", redirectUri},
                {"client_secret", Options.ClientSecret},
                {"code", code},
                {"grant_type", "authorization_code"}
            };

            var requestContent = new FormUrlEncodedContent(tokenRequestParameters);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = requestContent;
            var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                var _ = Options.CallbackPath;
                return OAuthTokenResponse.Success(payload);
            }

            var error = "OAuth token endpoint failure: " + await Display(response);
            return OAuthTokenResponse.Failed(new Exception(error));
        }

        protected async Task<OAuthTokenResponse> RefreshCodeAsync(string code)
        {
            var tokenRequestParameters = new Dictionary<string, string>
            {
                {"client_id", Options.ClientId},
                {"refresh_token", code},
                {"client_secret", Options.ClientSecret},
                {"grant_type", "refresh_token"}
            };

            var requestContent = new FormUrlEncodedContent(tokenRequestParameters);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = requestContent;
            var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                var _ = Options.CallbackPath;
                return OAuthTokenResponse.Success(payload);
            }

            var error = "OAuth token endpoint failure: " + await Display(response);
            return OAuthTokenResponse.Failed(new Exception(error));
        }


        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            AuthenticationProperties properties = null;
            var query = Request.Query;

            var error = query["error"];
            if (!StringValues.IsNullOrEmpty(error))
            {
                var failureMessage = new StringBuilder();
                failureMessage.Append(error);
                var errorDescription = query["error_description"];
                if (!StringValues.IsNullOrEmpty(errorDescription))
                    failureMessage.Append(";Description=").Append(errorDescription);
                var errorUri = query["error_uri"];
                if (!StringValues.IsNullOrEmpty(errorUri)) { failureMessage.Append(";Uri=").Append(errorUri); }

                return HandleRequestResult.Fail(failureMessage.ToString());
            }

            var code = query["code"];
            var state = query["state"];

            properties = Options.StateDataFormat.Unprotect(state);
            if (properties == null) { return HandleRequestResult.Fail("The oauth state was missing or invalid."); }

            // OAuth2 10.12 CSRF
            if (!ValidateCorrelationId(properties)) { return HandleRequestResult.Fail("Correlation failed."); }

            if (StringValues.IsNullOrEmpty(code)) { return HandleRequestResult.Fail("Code was not found."); }

            var tokens = await ExchangeCodeAsync(code, BuildRedirectUri(Options.CallbackPath));

            if (tokens.Error != null) { return HandleRequestResult.Fail(tokens.Error); }

            if (string.IsNullOrEmpty(tokens.AccessToken))
            {
                return HandleRequestResult.Fail("Failed to retrieve access token.");
            }

            var identity = new ClaimsIdentity(ClaimsIssuer);

            if (Options.SaveTokens)
            {
                var authTokens = new List<AuthenticationToken>
                {
                    new AuthenticationToken {Name = "access_token", Value = tokens.AccessToken}
                };

                if (!string.IsNullOrEmpty(tokens.RefreshToken))
                    authTokens.Add(new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken });

                if (!string.IsNullOrEmpty(tokens.TokenType))
                    authTokens.Add(new AuthenticationToken { Name = "token_type", Value = tokens.TokenType });

                if (!string.IsNullOrEmpty(tokens.ExpiresIn))
                    if (int.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out var value))
                    {
                        // https://www.w3.org/TR/xmlschema-2/#dateTime
                        // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                        var expiresAt = Clock.UtcNow + TimeSpan.FromSeconds(value);
                        authTokens.Add(new AuthenticationToken
                        {
                            Name = "expires_at",
                            Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                        });
                    }

                properties.StoreTokens(authTokens);
            }

            var ticket = await CreateTicketAsync(identity, properties, tokens);
            if (ticket != null)
            {
                return HandleRequestResult.Success(ticket);
            }
            return HandleRequestResult.Fail("Failed to retrieve user information from remote server.");
        }

        private static async Task<string> Display(HttpResponseMessage response)
        {
            var output = new StringBuilder();
            output.Append("Status: " + response.StatusCode + ";");
            output.Append("Headers: " + response.Headers + ";");
            output.Append("Body: " + await response.Content.ReadAsStringAsync() + ";");
            return output.ToString();
        }
    }
}