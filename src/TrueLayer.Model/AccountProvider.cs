using System;
using Newtonsoft.Json;

namespace TrueLayer.Model
{
    public partial class AccountProvider
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("provider_id")]
        public string ProviderId { get; set; }

        [JsonProperty("logo_uri")]
        public string LogoUri { get; set; }
    }
}