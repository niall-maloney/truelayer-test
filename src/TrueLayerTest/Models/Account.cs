using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrueLayer;
using TrueLayer.Model;

namespace TrueLayerTest.Model
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

    public static partial class Map
    {
        public static Account MapToAccount(this TrueLayer.Model.Account dto) => new Account
        {
            UpdateTimestamp = dto.UpdateTimestamp,
            AccountId = dto.AccountId,
            AccountType = dto.AccountType,
            DisplayName = dto.DisplayName,
            Currency = dto.Currency,
            AccountNumber = dto.AccountNumber.MapToAccountNumber(),
            Provider = dto.Provider.MapToAccountProvider()
        };
    }
}