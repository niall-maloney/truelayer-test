using System;
using Newtonsoft.Json;

namespace TrueLayer.Model
{

    public partial class AccountNumber
    {
        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("swift_bic")]
        public string SwiftBic { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("sort_code")]
        public string SortCode { get; set; }
    }
}