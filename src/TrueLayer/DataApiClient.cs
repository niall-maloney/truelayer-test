using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using TrueLayer.Model;

namespace TrueLayer
{
    public class DataApiClient : IDataApiClient
    {
        public DataApiClient(IHttpClientWrapper httpClient, IDistributedCache cache)
        {
            httpClient.BaseAddress = new Uri("https://api.truelayer.com/data/v1/");
            HttpClient = httpClient;
            Cache = cache;
        }

        private IHttpClientWrapper HttpClient { get; }
        private IDistributedCache Cache { get; }

        public async Task<List<Account>> GetAccounts(string token)
        {
            var endpoint = new Uri(HttpClient.BaseAddress, $"accounts");
            // Try getting the accounts from the cache
            var response = await Cache.GetStringAsync(endpoint + token);

            // If the cache response is null, then get the data from true layer api
            if (response == null)
            {
                response = await HttpClient.GetData(endpoint, token);

                // Store the result in the cache using the endpoint+token as the key
                await Cache.SetStringAsync(endpoint + token, response,
                    new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)});
            }

            return TrueLayerResults<Account>.FromJson(response).Results;
        }

        public async Task<List<Transaction>> GetAccountTransactions(string token, string accountId)
        {
            var endpoint = new Uri(HttpClient.BaseAddress, $"accounts/{accountId}/transactions");
            // Try getting the accounts from the cache
            var response = await Cache.GetStringAsync(endpoint + token);

            // If the cache response is null, then get the data from true layer api
            if (response == null)
            {
                response = await HttpClient.GetData(endpoint, token);

                // Store the result in the cache using the endpoint+token as the key
                await Cache.SetStringAsync(endpoint + token, response,
                    new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)});
            }

            return TrueLayerResults<Transaction>.FromJson(response).Results;
        }
    }
}