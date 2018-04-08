using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace TrueLayerTest
{
    public class TrueLayerTokenRefreshContext : ResultContext<TrueLayerOptions>
    {
        public TrueLayerTokenRefreshContext(
            ClaimsPrincipal principal, HttpContext context, AuthenticationScheme scheme,
            AuthenticationProperties properties, TrueLayerOptions options) : base(context, scheme, options)
        {
            Properties = properties;
            Principal = principal;
        }
    }
}