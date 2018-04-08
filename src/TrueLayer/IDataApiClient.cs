using System.Collections.Generic;
using System.Threading.Tasks;
using TrueLayer.Model;

namespace TrueLayer
{
    public interface IDataApiClient
    {
        Task<List<Account>> GetAccounts(string token);

        Task<List<Transaction>> GetAccountTransactions(string token, string accountId);
    }
}