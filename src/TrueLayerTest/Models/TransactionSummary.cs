using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrueLayer;
using TrueLayer.Model;

namespace TrueLayerTest.Model
{
    public partial class TransactionSummary
    {
        private double _min;
        private double _max;
        private double _avg;

        [JsonProperty("transaction_category")]
        public string TransactionCategory { get; set; }

        [JsonProperty("minimum_amount")]
        public double MinimumAmount { get => _min; set => _min = Math.Round(value, 2); }

        [JsonProperty("maxiumum_amount")]
        public double MaximumAmount { get => _max; set => _max = Math.Round(value, 2); }

        [JsonProperty("average_amount")]
        public double AverageAmount { get => _avg; set => _avg = Math.Round(value, 2); }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }    
    
    public partial class TransactionSummary
    {
        public static TransactionSummary FromJson(string json) => JsonConvert.DeserializeObject<TransactionSummary>(json, Converter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this TransactionSummary self) => JsonConvert.SerializeObject(self, Converter.Settings);


        public static string ToJson(this List<TransactionSummary> self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}