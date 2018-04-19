using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrueLayer;

namespace TrueLayerTest.Model
{
    public class Results : IResultsModel
    {
        private IDataApiClient DataApiClient { get; }

        public Results(IDataApiClient apiClient)
        {
            DataApiClient = apiClient;
        }

        public async Task<List<Account>> GetAccounts(string accessToken)
        {
            var response = await DataApiClient.GetAccounts(accessToken).ConfigureAwait(false);

            return response.Select(x => x.MapToAccount()).ToList();
        }

        public async Task<List<Transaction>> GetTransactions(string accessToken, string accountId)
        {
            var response = await DataApiClient.GetAccountTransactions(accessToken, accountId).ConfigureAwait(false);

            return response.Select(x => x.MapToTransaction()).ToList();
        }

        public async Task<GroupedTransactionResults> GetTransactionsGroupedByAccount(string accessToken)
        {
            var accounts = await GetAccounts(accessToken).ConfigureAwait(false);

            var groupedTransactions = new List<List<Transaction>>();
            foreach (var account in accounts)
            {
                groupedTransactions.Add(await GetTransactions(accessToken, account.AccountId).ConfigureAwait(false));
            }

            return new GroupedTransactionResults { Results = groupedTransactions };
        }

        public async Task<GroupedTransactionSummaryResults> GetTransactionSummaryGroupedByCategory(string accessToken)
        {
            var accounts = await GetAccounts(accessToken);

            var transactions = new List<Transaction>();
            foreach (var account in accounts)
            {
                transactions.AddRange(await GetTransactions(accessToken, account.AccountId).ConfigureAwait(false));
            }

            // Group the transactions by TransactionCateogry and Currency
            var transactionsGroupedByCategory = transactions.GroupBy(t => $"{t.TransactionCategory}|{t.Currency}");

            // Map them to TransactionSummary objects
            var transactionSummariesGroupedByCategory = transactionsGroupedByCategory.Select(t => new TransactionSummary
            {
                TransactionCategory = t.Key.Split('|')[0],
                MaximumAmount = t.Max(e => e.Amount),
                MinimumAmount = t.Min(e => e.Amount),
                AverageAmount = t.Average(e => e.Amount),
                Currency = t.Key.Split('|')[1]
            }).ToList();

            return new GroupedTransactionSummaryResults { Results = transactionSummariesGroupedByCategory };
        }

        public static T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json, Converter.Settings);
    }
    public class GroupedTransactionResults
    {
        [JsonProperty("results")]
        public List<List<Transaction>> Results { get; set; }
    }

    public class GroupedTransactionSummaryResults
    {
        [JsonProperty("results")]
        public List<TransactionSummary> Results { get; set; }
    }

    public static partial class Serialize
    {
        public static string ToJson(this GroupedTransactionResults self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string ToJson(this GroupedTransactionSummaryResults self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}