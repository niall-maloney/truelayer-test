using System;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueLayer.Model
{
    public partial class TrueLayerResults<T>
    {
        [JsonProperty("results")]
        public List<T> Results { get; set; }
    }

    public partial class TrueLayerResults<T>
    {
        public static TrueLayerResults<T> FromJson(string json) => JsonConvert.DeserializeObject<TrueLayerResults<T>>(json, Converter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson<T>(this TrueLayerResults<T> self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}