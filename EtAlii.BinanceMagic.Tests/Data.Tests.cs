namespace EtAlii.BinanceMagic.Tests
{
    using System.Linq;
    using System.Threading;
    using EtAlii.BinanceMagic.Circular;
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
            var output = new ConsoleOutput();
            var programSettings = _context.CreateProgramSettings();
            var algorithmSettings = _context.CreateCircularAlgorithmSettings();
            var program = new Program(programSettings, output);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            
            // Act.
            var data = new Data(client, algorithmSettings, output);
            
            // Assert.
            Assert.NotNull(data);
        }
        
        [Fact]
        public void Data_Load_Empty()
        {
            // Arrange.
            var output = new ConsoleOutput();
            var programSettings = _context.CreateProgramSettings();
            var algorithmSettings = _context.CreateCircularAlgorithmSettings();
            var program = new Program(programSettings, output);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            var data = new Data(client, algorithmSettings, output);
            
            // Act.
            data.Load();

            // Assert.
            Assert.Empty(data.Transactions);
        }
                
        [Fact]
        public void Data_Add_Transaction()
        {
            // Arrange.
            var output = new ConsoleOutput();
            var programSettings = _context.CreateProgramSettings();
            var algorithmSettings = _context.CreateCircularAlgorithmSettings();
            var program = new Program(programSettings, output);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            var data = new Data(client, algorithmSettings, output);
            data.Load();
            var firstCoin = algorithmSettings.AllowedCoins.First();
            var secondCoin = algorithmSettings.AllowedCoins.Skip(1).First();
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
            var output = new ConsoleOutput();
            var programSettings = _context.CreateProgramSettings();
            var algorithmSettings = _context.CreateCircularAlgorithmSettings();
            var program = new Program(programSettings, output);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            var data = new Data(client, algorithmSettings, output);
            data.Load();
            var firstCoin = algorithmSettings.AllowedCoins.First();
            var secondCoin = algorithmSettings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, 12, 2);
            
            // Act.
            data.AddTransaction(firstTransaction);
            data.AddTransaction(secondTransaction);

            // Assert.
            Assert.Equal(2, data.Transactions.Count);
            Assert.Equal(10, data.Transactions[0].Profit);
            Assert.Equal(firstCoin, data.Transactions[0].From.Symbol);
            Assert.Equal(secondCoin, data.Transactions[0].To.Symbol);
            Assert.Equal(12, data.Transactions[1].Profit);
            Assert.Equal(secondCoin, data.Transactions[1].From.Symbol);
            Assert.Equal(firstCoin, data.Transactions[1].To.Symbol);
        }
                        
        [Fact]
        public void Data_Add_Transactions_And_Reload()
        {
            // Arrange.
            var output = new ConsoleOutput();
            var programSettings = _context.CreateProgramSettings();
            var algorithmSettings = _context.CreateCircularAlgorithmSettings();
            var program = new Program(programSettings, output);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            var data = new Data(client, algorithmSettings, output);
            data.Load();
            var firstCoin = algorithmSettings.AllowedCoins.First();
            var secondCoin = algorithmSettings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, 12, 2);
            data.AddTransaction(firstTransaction);
            data.AddTransaction(secondTransaction);
            
            // Act.
            data = new Data(client, algorithmSettings, output);
            data.Load();

            // Assert.
            Assert.Equal(2, data.Transactions.Count);
            Assert.Equal(10, data.Transactions[0].Profit);
            Assert.Equal(firstCoin, data.Transactions[0].From.Symbol);
            Assert.Equal(secondCoin, data.Transactions[0].To.Symbol);
            Assert.Equal(12, data.Transactions[1].Profit);
            Assert.Equal(secondCoin, data.Transactions[1].From.Symbol);
            Assert.Equal(firstCoin, data.Transactions[1].To.Symbol);
        }

        
        [Fact]
        public void Data_Build_Targets()
        {
            // Arrange.
            var output = new ConsoleOutput();
            var programSettings = _context.CreateProgramSettings();
            var algorithmSettings = _context.CreateCircularAlgorithmSettings();
            var program = new Program(programSettings, output);
            var details = new TradeDetails();
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            var data = new Data(client, algorithmSettings, output);
            var targetBuilder = new TradeDetailsUpdater(data, algorithmSettings);
            data.Load();
            var firstCoin = algorithmSettings.AllowedCoins.First();
            var secondCoin = algorithmSettings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, algorithmSettings.InitialTarget, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, algorithmSettings.InitialTarget * (1 + algorithmSettings.TargetIncrease), 2);
            
            // Act.
            targetBuilder.UpdateTargetDetails(details);

            // Assert.
            Assert.Equal(firstCoin, details.SellCoin);
            Assert.Equal(secondCoin, details.BuyCoin);
            var firstGoal = details.Target;
            Assert.Equal(algorithmSettings.InitialTarget, firstGoal);

            data.AddTransaction(firstTransaction);
            targetBuilder.UpdateTargetDetails(details);

            Assert.Equal(secondCoin, details.SellCoin);
            Assert.Equal(firstCoin, details.BuyCoin);
            var secondGoal = details.Target;
            Assert.Equal(firstGoal * (1 + algorithmSettings.TargetIncrease), secondGoal);

            data.AddTransaction(secondTransaction);
            targetBuilder.UpdateTargetDetails(details);

            Assert.Equal(firstCoin, details.SellCoin);
            Assert.Equal(secondCoin, details.BuyCoin);
            var thirdGoal = details.Target;
            Assert.Equal(secondGoal * (1 + algorithmSettings.TargetIncrease), thirdGoal);
        }

        [Fact]
        public void Data_Situation_Get()
        {
            // Arrange.
            var output = new ConsoleOutput();
            var programSettings = _context.CreateProgramSettings();
            var algorithmSettings = _context.CreateCircularAlgorithmSettings();
            var program = new Program(programSettings, output);
            var details = new TradeDetails();
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            var data = new Data(client, algorithmSettings, output);
            var targetBuilder = new TradeDetailsUpdater(data, algorithmSettings);
            client.Start();
            data.Load();
            var firstCoin = algorithmSettings.AllowedCoins.First();
            var secondCoin = algorithmSettings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            data.AddTransaction(firstTransaction);
            var cancellationToken = CancellationToken.None;
            targetBuilder.UpdateTargetDetails(details);
            
            // Act.
            var result = data.TryGetSituation(details, cancellationToken, out Situation situation);

            // Assert.
            Assert.True(result);
            Assert.NotNull(situation);
            Assert.True(situation.IsInitialCycle);
        }
                       
        [Fact]
        public void Data_Situation_Get_After_InitialCycle()
        {
            // Arrange.
            var output = new ConsoleOutput();
            var programSettings = _context.CreateProgramSettings();
            var algorithmSettings = _context.CreateCircularAlgorithmSettings();
            var program = new Program(programSettings, output);
            var details = new TradeDetails();
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            var data = new Data(client, algorithmSettings, output);
            var targetBuilder = new TradeDetailsUpdater(data, algorithmSettings);
            client.Start();
            data.Load();
            var firstCoin = algorithmSettings.AllowedCoins.First();
            var secondCoin = algorithmSettings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, 12, 2);
            data.AddTransaction(firstTransaction);
            data.AddTransaction(secondTransaction);
            var cancellationToken = CancellationToken.None;
            targetBuilder.UpdateTargetDetails(details);
            
            // Act.
            var result = data.TryGetSituation(details, cancellationToken, out Situation situation);

            // Assert.
            Assert.True(result);
            Assert.NotNull(situation);
            Assert.False(situation.IsInitialCycle);
        }
    }
}