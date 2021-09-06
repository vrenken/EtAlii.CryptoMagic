namespace EtAlii.CryptoMagic.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EtAlii.CryptoMagic.Service;
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
            var actionValidator = new ActionValidator();
            
            // Act.
            var client = new Client(actionValidator)
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
            var actionValidator = new ActionValidator();
            var client = new Client(actionValidator)
            {
                PlaceTestOrders = true
            };
            
            // Act.
            client.Start("ApiKey", "SecretKey");
            
            // Assert.
            Assert.NotNull(client);
        }
                        
        [Fact(Skip = "Only for local testing")]
        public async Task Client_TryConvert()
        {
            // Arrange.
            var referenceSymbol = "USDT";
            var actionValidator = new ActionValidator();
            var client = new Client(actionValidator)
            {
                PlaceTestOrders = true
            };
            await client.Start("ApiKey", "SecretKey");
            var cancellationToken = CancellationToken.None;
            var transactionId = _context.Random.Next();
            var sellAction = new SellAction
            {
                Symbol = "ETH",
                Quantity = 0.0048m, // = ~11,143248 USD
                Price = 2321.51m,
                QuotedQuantity = 2321.51m * 0.0048m,
                TransactionId = $"{transactionId:000000}_0_ETH_BNB",
            };
            var buyAction = new BuyAction
            {
                Symbol = "BNB",
                Quantity = 0.03m, // = ~15,29325 BUSD
                Price = 509.7750m,
                QuotedQuantity = 509.7750m * 0.03m, 
                TransactionId = $"{transactionId:000000}_1_BNB_ETH",
            };
            
            // Act.
            var (success, transaction,error) = await client.TryConvert(sellAction, buyAction, referenceSymbol, cancellationToken, () => DateTime.Now);
            
            // Assert.
            Assert.True(success);
            Assert.NotNull(transaction);
            Assert.Null(error);
        }
    }
}