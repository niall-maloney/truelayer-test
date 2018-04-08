using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrueLayer;
using TrueLayer.Model;

namespace TrueLayerTest.Model
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

    public static partial class Map
    {
        public static AccountNumber MapToAccountNumber(this TrueLayer.Model.AccountNumber dto) => new AccountNumber
        {
            Iban = dto.Iban,
            SwiftBic = dto.SwiftBic,
            Number = dto.Number,
            SortCode = dto.SortCode
        };
    }
}