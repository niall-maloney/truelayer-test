using Microsoft.AspNetCore.Authentication.OAuth;

namespace TrueLayerTest
{
    public class TrueLayerOptions : OAuthOptions
    {
        public TrueLayerOptions()
        {
            Events = new TrueLayerAuthEvents();
        }

        public bool EnableMock { get; set; }

        public new TrueLayerAuthEvents Events
        {
            get => (TrueLayerAuthEvents) base.Events;
            set => base.Events = value;
        }
    }
}