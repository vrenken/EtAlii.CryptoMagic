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
            var programSettings = _context.CreateProgramSettings();
            var loopSettings = _context.CreateLoopSettings();
            var program = new Program(programSettings);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator);
            
            // Act.
            var data = new Data(client, loopSettings);
            
            // Assert.
            Assert.NotNull(data);
        }
        
        [Fact]
        public void Data_Load_Empty()
        {
            // Arrange.
            var programSettings = _context.CreateProgramSettings();
            var loopSettings = _context.CreateLoopSettings();
            var program = new Program(programSettings);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator);
            var data = new Data(client, loopSettings);
            
            // Act.
            data.Load();

            // Assert.
            Assert.Empty(data.Transactions);
        }
                
        [Fact]
        public void Data_Add_Transaction()
        {
            // Arrange.
            var programSettings = _context.CreateProgramSettings();
            var loopSettings = _context.CreateLoopSettings();
            var program = new Program(programSettings);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator);
            var data = new Data(client, loopSettings);
            data.Load();
            var firstCoin = loopSettings.AllowedCoins.First();
            var secondCoin = loopSettings.AllowedCoins.Skip(1).First();
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
            var programSettings = _context.CreateProgramSettings();
            var loopSettings = _context.CreateLoopSettings();
            var program = new Program(programSettings);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator);
            var data = new Data(client, loopSettings);
            data.Load();
            var firstCoin = loopSettings.AllowedCoins.First();
            var secondCoin = loopSettings.AllowedCoins.Skip(1).First();
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
            var programSettings = _context.CreateProgramSettings();
            var loopSettings = _context.CreateLoopSettings();
            var program = new Program(programSettings);
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator);
            var data = new Data(client, loopSettings);
            data.Load();
            var firstCoin = loopSettings.AllowedCoins.First();
            var secondCoin = loopSettings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, 10, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, 12, 2);
            data.AddTransaction(firstTransaction);
            data.AddTransaction(secondTransaction);
            
            // Act.
            data = new Data(client, loopSettings);
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
            var programSettings = _context.CreateProgramSettings();
            var loopSettings = _context.CreateLoopSettings();
            var program = new Program(programSettings);
            var details = new TradeDetails();
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator);
            var data = new Data(client, loopSettings);
            var targetBuilder = new TradeDetailsUpdater(data, loopSettings);
            data.Load();
            var firstCoin = loopSettings.AllowedCoins.First();
            var secondCoin = loopSettings.AllowedCoins.Skip(1).First();
            var firstTransaction = _context.CreateTransaction(firstCoin, 10, 2, secondCoin, 5, 1, loopSettings.MinimalTargetProfit, 2);
            var secondTransaction = _context.CreateTransaction(secondCoin, 2, 5, firstCoin, 2, 5, loopSettings.MinimalTargetProfit * (1 + loopSettings.MinimalIncrease), 2);
            
            // Act.
            targetBuilder.UpdateTargetDetails(details);

            // Assert.
            Assert.Equal(firstCoin, details.FromCoin);
            Assert.Equal(secondCoin, details.ToCoin);
            var firstGoal = details.Goal;
            Assert.Equal(loopSettings.MinimalTargetProfit, firstGoal);

            data.AddTransaction(firstTransaction);
            targetBuilder.UpdateTargetDetails(details);

            Assert.Equal(secondCoin, details.FromCoin);
            Assert.Equal(firstCoin, details.ToCoin);
            var secondGoal = details.Goal;
            Assert.Equal(firstGoal * (1 + loopSettings.MinimalIncrease), secondGoal);

            data.AddTransaction(secondTransaction);
            targetBuilder.UpdateTargetDetails(details);

            Assert.Equal(firstCoin, details.FromCoin);
            Assert.Equal(secondCoin, details.ToCoin);
            var thirdGoal = details.Goal;
            Assert.Equal(secondGoal * (1 + loopSettings.MinimalIncrease), thirdGoal);
        }

        [Fact]
        public void Data_Situation_Get()
        {
            // Arrange.
            var programSettings = _context.CreateProgramSettings();
            var loopSettings = _context.CreateLoopSettings();
            var program = new Program(programSettings);
            var details = new TradeDetails();
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator);
            var data = new Data(client, loopSettings);
            var targetBuilder = new TradeDetailsUpdater(data, loopSettings);
            client.Start();
            data.Load();
            var firstCoin = loopSettings.AllowedCoins.First();
            var secondCoin = loopSettings.AllowedCoins.Skip(1).First();
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
            var programSettings = _context.CreateProgramSettings();
            var loopSettings = _context.CreateLoopSettings();
            var program = new Program(programSettings);
            var details = new TradeDetails();
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator);
            var data = new Data(client, loopSettings);
            var targetBuilder = new TradeDetailsUpdater(data, loopSettings);
            client.Start();
            data.Load();
            var firstCoin = loopSettings.AllowedCoins.First();
            var secondCoin = loopSettings.AllowedCoins.Skip(1).First();
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