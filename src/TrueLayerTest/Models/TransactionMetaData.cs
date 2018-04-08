using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrueLayer;
using TrueLayer.Model;

namespace TrueLayerTest.Model
{
    public partial class TransactionMetaData
    {
        [JsonProperty("bank_transaction_id")]
        public string BankTransactionId { get; set; }

        [JsonProperty("provider_transaction_category")]
        public string ProviderTransactionCategory { get; set; }
    }
    public static partial class Map
    {
        public static TransactionMetaData MapToTransactionMetaData(this TrueLayer.Model.TransactionMetaData dto) => new TransactionMetaData
        {
            BankTransactionId = dto.BankTransactionId,
            ProviderTransactionCategory = dto.ProviderTransactionCategory
        };
    }
}