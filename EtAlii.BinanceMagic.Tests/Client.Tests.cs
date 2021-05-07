namespace EtAlii.BinanceMagic.Tests
{
    using System;
    using System.Threading;
    using EtAlii.BinanceMagic.Circular;
    using Xunit;

    public class ClientTests : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public ClientTests(TestContext context)
        {
            _context = context;
        }
        
        [Fact]
        public void Client_Create()
        {
            // Arrange.
            var output = new ConsoleOutput();
            var settings = _context.CreateProgramSettings();
            var program = new Program(settings, output);
            var actionValidator = new ActionValidator();
            
            // Act.
            var client = new Client(settings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            
            // Assert.
            Assert.NotNull(client);
        }
                
        [Fact]
        public void Client_Start()
        {
            // Arrange.
            var output = new ConsoleOutput();
            var settings = _context.CreateProgramSettings();
            var program = new Program(settings, output);
            var actionValidator = new ActionValidator();
            var client = new Client(settings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            
            // Act.
            client.Start();
            
            // Assert.
            Assert.NotNull(client);
        }
                        
        [Fact]
        public void Client_TryConvert()
        {
            // Arrange.
            var output = new ConsoleOutput();
            var programSettings = _context.CreateProgramSettings();
            var algorithmSettings = _context.CreateCircularAlgorithmSettings();
            var program = new Program(programSettings, output);
            var status = new TradeDetails();
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = true
            };
            client.Start();
            var cancellationToken = CancellationToken.None;
            var transactionId = _context.Random.Next();
            var sellAction = new SellAction
            {
                Coin = "ETH",
                Quantity = 0.0048m, // = ~11,143248 USD
                UnitPrice = 2321.51m,
                Price = 2321.51m * 0.0048m,
                TransactionId = $"{transactionId:000000}_0_ETH_BNB",
            };
            var buyAction = new BuyAction
            {
                Coin = "BNB",
                Quantity = 0.03m, // = ~15,29325 BUSD
                UnitPrice = 509.7750m,
                Price = 509.7750m * 0.03m, 
                TransactionId = $"{transactionId:000000}_1_BNB_ETH",
            };
            
            // Act.
            client.TryConvert(sellAction, buyAction, algorithmSettings.ReferenceCoin, status, cancellationToken, () => DateTime.Now, out var transaction);
            
            // Assert.
            Assert.NotNull(transaction);
        }
    }
}