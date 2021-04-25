namespace EtAlii.BinanceMagic.Tests
{
    using System.Linq;
    using System.Threading;
    using Xunit;

    public class DataTests : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public DataTests(TestContext context)
        {
            _context = context;
        }
        
        [Fact]
        public void Data_Create()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            
            // Act.
            var data = new Data(client, settings);
            
            // Assert.
            Assert.NotNull(data);
        }
        
        [Fact]
        public void Data_Load_Empty()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            var data = new Data(client, settings);
            
            // Act.
            data.Load();

            // Assert.
            Assert.Empty(data.Transactions);
        }
                
        [Fact]
        public void Data_Add_Transaction()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            var data = new Data(client, settings);
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
        public void Data_Add_Transactions()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            var data = new Data(client, settings);
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
            Assert.Equal(10, data.Transactions[0].TotalProfit);
            Assert.Equal(firstCoin, data.Transactions[0].From.Coin);
            Assert.Equal(secondCoin, data.Transactions[0].To.Coin);
            Assert.Equal(12, data.Transactions[1].TotalProfit);
            Assert.Equal(secondCoin, data.Transactions[1].From.Coin);
            Assert.Equal(firstCoin, data.Transactions[1].To.Coin);
        }
                        
        [Fact]
        public void Data_Add_Transactions_And_Reload()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            var data = new Data(client, settings);
            data.Load();
            var firstCoin = settings.AllowedCoins.First();
            var secondCoin = settings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, 12, 2);
            data.AddTransaction(firstTransaction);
            data.AddTransaction(secondTransaction);
            
            // Act.
            data = new Data(client, settings);
            data.Load();

            // Assert.
            Assert.Equal(2, data.Transactions.Count);
            Assert.Equal(10, data.Transactions[0].TotalProfit);
            Assert.Equal(firstCoin, data.Transactions[0].From.Coin);
            Assert.Equal(secondCoin, data.Transactions[0].To.Coin);
            Assert.Equal(12, data.Transactions[1].TotalProfit);
            Assert.Equal(secondCoin, data.Transactions[1].From.Coin);
            Assert.Equal(firstCoin, data.Transactions[1].To.Coin);
        }

        
        [Fact]
        public void Data_Build_Targets()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            var data = new Data(client, settings);
            data.Load();
            var firstCoin = settings.AllowedCoins.First();
            var secondCoin = settings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, settings.MinimalTargetProfit, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, settings.MinimalTargetProfit * (1 + settings.MinimalIncrease), 2);
            
            // Act.
            var firstTarget = data.BuildTarget();
            data.AddTransaction(firstTransaction);
            var secondTarget = data.BuildTarget();
            data.AddTransaction(secondTransaction);
            var thirdTarget = data.BuildTarget();

            // Assert.
            Assert.Equal(firstCoin, firstTarget.Source);
            Assert.Equal(secondCoin, firstTarget.Destination);
            Assert.Equal(settings.MinimalTargetProfit, firstTarget.Profit);
            
            Assert.Equal(secondCoin, secondTarget.Source);
            Assert.Equal(firstCoin, secondTarget.Destination);
            Assert.Equal(firstTarget.Profit * (1 + settings.MinimalIncrease), secondTarget.Profit);

            Assert.Equal(firstCoin, thirdTarget.Source);
            Assert.Equal(secondCoin, thirdTarget.Destination);
            Assert.Equal(secondTarget.Profit * (1 + settings.MinimalIncrease), thirdTarget.Profit);
        }

        [Fact]
        public void Data_Situation_Get()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            client.Start();
            var data = new Data(client, settings);
            data.Load();
            var firstCoin = settings.AllowedCoins.First();
            var secondCoin = settings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            data.AddTransaction(firstTransaction);
            var cancellationToken = CancellationToken.None;
            var target = data.BuildTarget();
            
            // Act.
            var situation = data.GetSituation(target, cancellationToken);

            // Assert.
            Assert.NotNull(situation);
            Assert.True(situation.IsInitialCycle);
        }
                       
        [Fact]
        public void Data_Situation_Get_After_InitialCycle()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            client.Start();
            var data = new Data(client, settings);
            data.Load();
            var firstCoin = settings.AllowedCoins.First();
            var secondCoin = settings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, 12, 2);
            data.AddTransaction(firstTransaction);
            data.AddTransaction(secondTransaction);
            var cancellationToken = CancellationToken.None;
            var target = data.BuildTarget();
            
            // Act.
            var situation = data.GetSituation(target, cancellationToken);

            // Assert.
            Assert.NotNull(situation);
            Assert.False(situation.IsInitialCycle);
        }
    }
}