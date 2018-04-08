using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrueLayer.Model
{
    public partial class TransactionMetaData
    {
        [JsonProperty("bank_transaction_id")]
        public string BankTransactionId { get; set; }

        [JsonProperty("provider_transaction_category")]
        public string ProviderTransactionCategory { get; set; }
    }
}