using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrueLayer;
using TrueLayer.Model;

namespace TrueLayerTest.Model
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
    
    public static partial class Map
    {
        public static AccountProvider MapToAccountProvider(this TrueLayer.Model.AccountProvider dto) => new AccountProvider
        {
            DisplayName = dto.DisplayName,
            ProviderId = dto.ProviderId,
            LogoUri = dto.LogoUri
        };
    }
}