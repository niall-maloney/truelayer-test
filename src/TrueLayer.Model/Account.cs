using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueLayer.Model
{
    public partial class Account
    {
        [JsonProperty("update_timestamp")]
        public DateTimeOffset UpdateTimestamp { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("account_type")]
        public string AccountType { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("account_number")]
        public AccountNumber AccountNumber { get; set; }

        [JsonProperty("provider")]
        public AccountProvider Provider { get; set; }
    }

    public partial class Account
    {
        public static Account FromJson(string json) => JsonConvert.DeserializeObject<Account>(json, Converter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this Account self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
