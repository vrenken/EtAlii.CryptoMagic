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
            
            // Act.
            var client = new Client(settings);
            
            // Assert.
            Assert.NotNull(client);
        }
                
        [Fact]
        public void Client_Start()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var client = new Client(settings);
            
            // Act.
            client.Start();
            
            // Assert.
            Assert.NotNull(client);
        }
                        
        [Fact]
        public void Client_TryConvert()
        {
            // Arrange.
            var settings = _context.CreateSettings();
            var client = new Client(settings);
            client.Start();
            var cancellationToken = CancellationToken.None;
            var transactionId = _context.Random.Next();
            var sellAction = new SellAction
            {
                Coin = "ETH",
                Quantity = 0.0001200m, // = ~ 0,027 USD
                TargetPrice = 0.027m,
                TransactionId = $"{transactionId:000000}_0_ETH_BNB",
            };
            var buyAction = new BuyAction
            {
                Coin = "BNB",
                Quantity = 0.001m, // = ~ 0.48439623070802 BUSD
                TargetPrice = 0.48439623070802m,
                TransactionId = $"{transactionId:000000}_1_BNB_ETH",
            };
            
            // Act.
            client.TryConvert(sellAction, buyAction, cancellationToken, out var transaction);
            
            // Assert.
            Assert.NotNull(transaction);
        }
    }
}