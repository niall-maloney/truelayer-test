using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrueLayer.Model
{

    public partial class Transaction
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("transaction_type")]
        public string TransactionType { get; set; }

        [JsonProperty("transaction_category")]
        public string TransactionCategory { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("meta")]
        public TransactionMetaData MetaData { get; set; }
    }

    public partial class Transaction
    {
        public static Transaction FromJson(string json) => JsonConvert.DeserializeObject<Transaction>(json, Converter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this Transaction self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
