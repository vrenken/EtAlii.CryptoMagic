namespace EtAlii.BinanceMagic.Tests
{
    using System.Threading;
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
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            
            // Act.
            var client = new Client(settings, program);
            
            // Assert.
            Assert.NotNull(client);
        }
                
        [Fact]
        public void Client_Start()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            
            // Act.
            client.Start();
            
            // Assert.
            Assert.NotNull(client);
        }
                        
        [Fact(Skip = "Live test")]
        public void Client_TryConvert()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var program = new Program(settings);
            var client = new Client(settings, program);
            client.Start();
            var cancellationToken = CancellationToken.None;
            var transactionId = _context.Random.Next();
            var sellAction = new SellAction
            {
                Coin = "ETH",
                Quantity = 0.0048m, // = ~11,23 USD
                TargetPrice = 2321.51m,
                TransactionId = $"{transactionId:000000}_0_ETH_BNB",
            };
            var buyAction = new BuyAction
            {
                Coin = "BNB",
                Quantity = 0.02m, // = ~10.17 BUSD
                TargetPrice = 509.7750m,
                TransactionId = $"{transactionId:000000}_1_BNB_ETH",
            };
            
            // Act.
            client.TryConvert(sellAction, buyAction, cancellationToken, out var transaction);
            
            // Assert.
            Assert.NotNull(transaction);
        }
    }
}