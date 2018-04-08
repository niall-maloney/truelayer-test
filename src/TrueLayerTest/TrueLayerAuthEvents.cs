using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace TrueLayerTest
{
    public class TrueLayerAuthEvents : OAuthEvents
    {
        public Func<TrueLayerTokenRefreshContext, Task> OnRefreshToken { get; set; } = context => Task.CompletedTask;

        public Task RefreshToken(TrueLayerTokenRefreshContext context) => OnRefreshToken(context);
    }
}