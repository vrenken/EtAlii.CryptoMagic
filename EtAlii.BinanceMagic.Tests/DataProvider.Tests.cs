namespace EtAlii.BinanceMagic.Tests
{
    using System.Linq;
    using System.Threading;
    using Xunit;

    public class DataProviderTests : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public DataProviderTests(TestContext context)
        {
            _context = context;
        }
        
        [Fact]
        public void DataProvider_Create()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var client = new Client(settings);
            
            // Act.
            var data = new DataProvider(client, settings);
            
            // Assert.
            Assert.NotNull(data);
        }
        
        [Fact]
        public void DataProvider_Load_Empty()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var client = new Client(settings);
            var data = new DataProvider(client, settings);
            
            // Act.
            data.Load();

            // Assert.
            Assert.Empty(data.Transactions);
        }
                
        [Fact]
        public void DataProvider_Add_Transaction()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var client = new Client(settings);
            var data = new DataProvider(client, settings);
            data.Load();
            var firstCoin = settings.AllowedCoins.First();
            var secondCoin = settings.AllowedCoins.Skip(1).First();
            var transaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            
            // Act.
            data.AddTransaction(transaction);

            // Assert.
            Assert.Equal(1, data.Transactions.Count);
        }
                        
        [Fact]
        public void DataProvider_Add_Transactions()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var client = new Client(settings);
            var data = new DataProvider(client, settings);
            data.Load();
            var firstCoin = settings.AllowedCoins.First();
            var secondCoin = settings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, 12, 2);
            
            // Act.
            data.AddTransaction(firstTransaction);
            data.AddTransaction(secondTransaction);

            // Assert.
            Assert.Equal(2, data.Transactions.Count);
        }
                       
        [Fact]
        public void DataProvider_Situation_Get()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var client = new Client(settings);
            client.Start();
            var data = new DataProvider(client, settings);
            data.Load();
            var firstCoin = settings.AllowedCoins.First();
            var secondCoin = settings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            data.AddTransaction(firstTransaction);
            var cancellationToken = new CancellationToken();
            var target = data.BuildTarget(cancellationToken);
            
            // Act.
            var situation = data.GetSituation(target, cancellationToken);

            // Assert.
            Assert.NotNull(situation);
            Assert.True(situation.IsInitialCycle);
        }
                       
        [Fact]
        public void DataProvider_Situation_Get_After_InitialCycle()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var client = new Client(settings);
            client.Start();
            var data = new DataProvider(client, settings);
            data.Load();
            var firstCoin = settings.AllowedCoins.First();
            var secondCoin = settings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, 12, 2);
            data.AddTransaction(firstTransaction);
            data.AddTransaction(secondTransaction);
            var cancellationToken = new CancellationToken();
            var target = data.BuildTarget(cancellationToken);
            
            // Act.
            var situation = data.GetSituation(target, cancellationToken);

            // Assert.
            Assert.NotNull(situation);
            Assert.False(situation.IsInitialCycle);
        }
    }
}