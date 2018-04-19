using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TrueLayer;
using TrueLayerTest.Model;

namespace TrueLayerTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpClientWrapper>(provider => new HttpClientWrapper(new HttpClient()));
            services.AddSingleton<IDataApiClient, DataApiClient>();
            services.AddSingleton<IResultsModel, Results>();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "TrueLayer";
                })
                .AddCookie()
                .AddTrueLayerAuth("TrueLayer", options =>
                {
                    options.ClientId = Configuration["TRUELAYER_CLIENT_ID"];
                    options.ClientSecret = Configuration["TRUELAYER_CLIENT_SECRET"];
                    options.CallbackPath = new PathString("/signin-truelayer-token");
                    options.AuthorizationEndpoint = "https://auth.truelayer.com";
                    options.TokenEndpoint = "https://auth.truelayer.com/connect/token";
                    options.Scope.Add("accounts");
                    options.Scope.Add("transactions");
                    options.Scope.Add("offline_access");
                    options.EnableMock = true;
                    options.SaveTokens = true;

                    options.Events = new TrueLayerAuthEvents
                    {
                        OnRefreshToken = HandleOnRefreshTokenAsync
                    };
                });
            services.AddApiVersioning();
            services.AddMvc();
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration["CACHE_HOST"];
                option.InstanceName = Configuration["CACHE_NAME"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory log)
        {
            log.AddConsole(LogLevel.Trace);
            log.AddDebug();

            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UseAuthentication();
            app.UseMvc();
        }

        // This method handles the refreshing of the true layer api access token
        private static async Task HandleOnRefreshTokenAsync(TrueLayerTokenRefreshContext context)
        {
            var refreshToken = context.Properties.GetTokenValue("refresh_token");

            var httpContext = context.HttpContext;
            var options = context.Options;
            var properties = context.Properties;

            var pairs = new Dictionary<string, string>
            {
                {"client_id", options.ClientId},
                {"client_secret", options.ClientSecret},
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken}
            };

            var content = new FormUrlEncodedContent(pairs);
            var refreshResponse =
                await options.Backchannel.PostAsync(options.TokenEndpoint, content, httpContext.RequestAborted).ConfigureAwait(false);
            refreshResponse.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await refreshResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Persist the new acess token
            properties.UpdateTokenValue("access_token", payload.Value<string>("access_token"));
            refreshToken = payload.Value<string>("refresh_token");
            if (!string.IsNullOrEmpty(refreshToken)) { properties.UpdateTokenValue("refresh_token", refreshToken); }
            if (int.TryParse(payload.Value<string>("expires_in"), NumberStyles.Integer, CultureInfo.InvariantCulture,
                out var seconds))
            {
                var expiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(seconds);
                properties.UpdateTokenValue("expires_at", expiresAt.ToString("o", CultureInfo.InvariantCulture));
            }

            await httpContext.SignInAsync(context.Principal, properties).ConfigureAwait(false);
        }
    }
}