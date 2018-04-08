using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;

namespace TrueLayer.Tests
{
    public class DataApiClientTests
    {
        private readonly Uri _baseUri = new Uri("https://api.truelayer.com/data/v1");
        private readonly string _executingLocation = TestContext.CurrentContext.TestDirectory;

        private Mock<IHttpClientWrapper> SetupMockHttpClient(string endpointPath, string responseJson)
        {
            var mockHttpClient = new Mock<IHttpClientWrapper>();
            mockHttpClient.Setup(m => m.BaseAddress).Returns(_baseUri);
            mockHttpClient.Setup(m => m.GetData(new Uri(_baseUri, endpointPath), "token")).ReturnsAsync(responseJson);

            return mockHttpClient;
        }

        private Mock<IDistributedCache> SetupMockCache(string endpointPath, string responseJson)
        {
            var endpoint = new Uri(_baseUri, endpointPath).ToString();
            var responseBytes = Encoding.ASCII.GetBytes(responseJson);
            var mockCache = new Mock<IDistributedCache>();
            mockCache.Setup(m => m.GetAsync(endpoint + "token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseBytes);

            return mockCache;
        }

        private Mock<IDistributedCache> SetupNullMockCache()
        {
            var mockCache = new Mock<IDistributedCache>();
            mockCache.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]) null);

            return mockCache;
        }

        [Test]
        public void GetAccounts_ShouldReturn_ListOfAccounts()
        {
            // Arrange
            // Read in response data from file
            var dataPath = Path.Combine(_executingLocation, "..", "..", "..", "..", "..", "data",
                "mock-accounts-response.json");
            var responseJson = File.ReadAllText(dataPath);

            // Setup mocks
            var mockHttpClient = SetupMockHttpClient($"accounts", responseJson);
            var mockCache = SetupNullMockCache();

            // Instansiate TrueLayer DataApiClient
            var dataApiClient = new DataApiClient(mockHttpClient.Object, mockCache.Object);

            // Act
            var accounts = dataApiClient.GetAccounts("token").GetAwaiter().GetResult();

            // Assert
            // HttpClient should be called
            mockHttpClient.Verify(m => m.GetData(It.IsAny<Uri>(), It.IsAny<string>()), Times.AtLeastOnce);
            // Cache get should be called
            mockCache.Verify(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            // Cache set should be called
            mockCache.Verify(
                m => m.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()), Times.AtLeastOnce);


            Assert.AreEqual(2, accounts.Count);
        }

        [Test]
        public void GetTransactions_ShouldReturn_ListOfTransactions()
        {
            // Arrange
            // Read in response data from file
            var dataPath = Path.Combine(_executingLocation, "..", "..", "..", "..", "..", "data",
                "mock-transaction-response.json");
            var responseJson = File.ReadAllText(dataPath);

            // Fake acount id
            const string accountId = "fake-account-id";

            // Setup mocks
            var mockHttpClient = SetupMockHttpClient($"accounts/{accountId}/transactions", responseJson);
            var mockCache = SetupNullMockCache();

            // Instansiate TrueLayer DataApiClient
            var dataApiClient = new DataApiClient(mockHttpClient.Object, mockCache.Object);

            // Act
            var transactions = dataApiClient.GetAccountTransactions("token", accountId).GetAwaiter().GetResult();

            // Assert
            // HttpClient should be called
            mockHttpClient.Verify(m => m.GetData(It.IsAny<Uri>(), It.IsAny<string>()), Times.AtLeastOnce);
            // Cache get should be called
            mockCache.Verify(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            // Cache set should be called
            mockCache.Verify(
                m => m.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()), Times.AtLeastOnce);

            Assert.AreEqual(6, transactions.Count);
            Assert.AreEqual("c9ce76686887de57c5fdf67451303ed1", transactions[0].TransactionId);
            Assert.AreEqual("039ccc24c3", transactions[0].MetaData.BankTransactionId);
        }

        [Test]
        public void GetCachedAccounts_ShouldReturn_ListOfAccounts()
        {
            // Arrange
            // Read in response data from file
            var dataPath = Path.Combine(_executingLocation, "..", "..", "..", "..", "..", "data",
                "mock-accounts-response.json");
            var responseJson = File.ReadAllText(dataPath);

            // Setup mocks
            var mockHttpClient = SetupMockHttpClient($"accounts", responseJson);
            var mockCache = SetupMockCache($"accounts", responseJson);

            // Instansiate TrueLayer DataApiClient
            var dataApiClient = new DataApiClient(mockHttpClient.Object, mockCache.Object);

            // Act
            var accounts = dataApiClient.GetAccounts("token").GetAwaiter().GetResult();

            // Assert
            // HttpClient should not be called
            mockHttpClient.Verify(m => m.GetData(It.IsAny<Uri>(), It.IsAny<string>()), Times.Never);
            // Cache get should be called
            mockCache.Verify(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            // Cache set should not be called
            mockCache.Verify(
                m => m.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()), Times.Never);

            Assert.AreEqual(2, accounts.Count);
        }

        [Test]
        public void GetCachedTransactions_ShouldReturn_ListOfTransactions()
        {
            // Arrange
            // Read in response data from file
            var dataPath = Path.Combine(_executingLocation, "..", "..", "..", "..", "..", "data",
                "mock-transaction-response.json");
            var responseJson = File.ReadAllText(dataPath);

            // Fake acount id
            var accountId = "fake-account-id";

            // Setup mocks
            var mockHttpClient = SetupMockHttpClient($"accounts/{accountId}/transactions", responseJson);
            var mockCache = SetupMockCache($"accounts/{accountId}/transactions", responseJson);

            // Instansiate TrueLayer DataApiClient
            var dataApiClient = new DataApiClient(mockHttpClient.Object, mockCache.Object);

            // Act
            var transactions = dataApiClient.GetAccountTransactions("token", accountId).GetAwaiter().GetResult();

            // Assert
            // HttpClient should not be called
            mockHttpClient.Verify(m => m.GetData(It.IsAny<Uri>(), It.IsAny<string>()), Times.Never);
            // Cache get should be called
            mockCache.Verify(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            // Cache set should not be called
            mockCache.Verify(
                m => m.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()), Times.Never);

            Assert.AreEqual(6, transactions.Count);
            Assert.AreEqual("c9ce76686887de57c5fdf67451303ed1", transactions[0].TransactionId);
            Assert.AreEqual("039ccc24c3", transactions[0].MetaData.BankTransactionId);
        }
    }
}