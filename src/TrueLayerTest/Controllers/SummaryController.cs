using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using TrueLayer;
using TrueLayerTest.Model;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace TrueLayerTest.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/transactions/[controller]")]
    public class SummaryController : Controller
    {

        public SummaryController(IDistributedCache cache, IResultsModel results)
        {
            Cache = cache;
            Results = results;
        }

        public IDistributedCache Cache { get; }
        public IResultsModel Results { get; }

        // GET api/v1/transactions/summary
        [HttpGet]
        public async Task<string> GetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return "{}";
            }

            try
            {
                var accessToken = await RefreshTokenAsync().ConfigureAwait(false);

                var results = await Results.GetTransactionSummaryGroupedByCategory(accessToken).ConfigureAwait(false);

                return results.ToJson();
            }
            catch (HttpRequestException httpRequestException)
            {
                return $"Error getting data from TrueLayer API: {httpRequestException.Message}";
            }
        }

        private async Task<string> RefreshTokenAsync()
        {
            var userResult = await HttpContext.AuthenticateAsync("TrueLayer").ConfigureAwait(false);
            var properties = userResult.Properties;
            var expiry = properties.GetTokenValue("expires_at");
            if (DateTime.Parse(expiry) <= DateTime.Now)
            {
                var options = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<TrueLayerOptions>>().Get("TrueLayer");
                var scheme = await HttpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>().GetSchemeAsync("TrueLayer").ConfigureAwait(false);

                await options.Events.RefreshToken(new TrueLayerTokenRefreshContext(userResult.Principal, HttpContext, scheme, properties, options)).ConfigureAwait(false);
            }

            return properties.GetTokenValue("access_token");
        }
    }
}
