using System;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using TrueLayer;
using TrueLayer.Model;
using TrueLayerTest.Model;
using Account = TrueLayer.Model.Account;
using Transaction = TrueLayer.Model.Transaction;

namespace TrueLayerTest.Tests
{
    public class ResultsTests
    {
        private readonly string _executingLocation = TestContext.CurrentContext.TestDirectory;

        [Test]
        public void GetTransactionsGroupedByAccount_Should_ReturnGroupedTransactions()
        {
            // Arrange
            // Read in response data from file
            var accountDataPath = Path.Combine(_executingLocation, "..", "..", "..", "..", "..", "data",
                "mock-accounts-response.json");
            var accountsJson = File.ReadAllText(accountDataPath);
            var transactionDataPath = Path.Combine(_executingLocation, "..", "..", "..", "..", "..", "data",
                "mock-transaction-response.json");
            var transactionJson = File.ReadAllText(transactionDataPath);

            // Deserialise test data to objects
            var accounts = TrueLayerResults<Account>.FromJson(accountsJson).Results;
            var transactions = TrueLayerResults<Transaction>.FromJson(transactionJson).Results;

            // Setup mock TrueLayer DataApiClient
            var mockClient = new Mock<IDataApiClient>();
            mockClient.Setup(m => m.GetAccounts("token")).ReturnsAsync(accounts);
            mockClient.Setup(m => m.GetAccountTransactions("token", It.IsAny<string>())).ReturnsAsync(transactions);

            var resultsModel = new Results(mockClient.Object);

            // Act
            var result = resultsModel.GetTransactionsGroupedByAccount("token").GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(2, result.Results.Count);
            Assert.AreEqual(6, result.Results[0].Count);
        }

        [Test]
        public void GetTransactionSummaryGroupedByCategory_Should_ReturnGroupedTransactionSummaries()
        {
            // Arrange
            // Read in response data from file
            var accountDataPath = Path.Combine(_executingLocation, "..", "..", "..", "..", "..", "data",
                "mock-accounts-response.json");
            var accountsJson = File.ReadAllText(accountDataPath);
            var transactionDataPath = Path.Combine(_executingLocation, "..", "..", "..", "..", "..", "data",
                "mock-transaction-response.json");
            var transactionJson = File.ReadAllText(transactionDataPath);

            // Deserialise test data to objects
            var accounts = TrueLayerResults<Account>.FromJson(accountsJson).Results;
            var transactions = TrueLayerResults<Transaction>.FromJson(transactionJson).Results;

            // Setup mock TrueLayer DataApiClient
            var mockClient = new Mock<IDataApiClient>();
            mockClient.Setup(m => m.GetAccounts("token")).ReturnsAsync(accounts);
            mockClient.Setup(m => m.GetAccountTransactions("token", It.IsAny<string>())).ReturnsAsync(transactions);

            var resultsModel = new Results(mockClient.Object);

            // Act
            var result = resultsModel.GetTransactionSummaryGroupedByCategory("token").GetAwaiter().GetResult();

            Console.WriteLine(result.ToJson());

            // Assert
            Assert.AreEqual(4, result.Results.Count);
            Assert.AreEqual(-68.73,
                result.Results.FirstOrDefault(e => e.TransactionCategory == "DIRECT_DEBIT").AverageAmount);
        }
    }
}