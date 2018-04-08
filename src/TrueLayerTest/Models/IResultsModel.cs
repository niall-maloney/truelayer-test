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
    public interface IResultsModel
    {
        Task<GroupedTransactionResults> GetTransactionsGroupedByAccount(string accessToken);

        Task<GroupedTransactionSummaryResults> GetTransactionSummaryGroupedByCategory(string accessToken);

        Task<List<Account>> GetAccounts(string accessToken);

        Task<List<Transaction>> GetTransactions(string accessToken, string accountId);
    }
}