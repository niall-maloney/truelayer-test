using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace TrueLayerTest
{
    public static class TrueLayerAuthExtensions
    {
        public static AuthenticationBuilder AddTrueLayerAuth(this AuthenticationBuilder builder,
            string authenticationScheme, Action<TrueLayerOptions> configureOptions)
        {
            return builder.AddOAuth<TrueLayerOptions, TrueLayerAuthHandler>(authenticationScheme, configureOptions);
        }
    }
}