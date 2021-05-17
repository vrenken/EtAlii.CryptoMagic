namespace EtAlii.BinanceMagic.Tests
{
    using System;
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
            var persistence = new Persistence<TradeDetails>(programSettings, Guid.NewGuid().ToString());
            var client = new Client(program, actionValidator)
            {
                PlaceTestOrders = true
            };

            // Act.
            var data = new Data(client, algorithmSettings, persistence);

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
            var persistence = new Persistence<TradeDetails>(programSettings, Guid.NewGuid().ToString());
            var client = new Client(program, actionValidator)
            {
                PlaceTestOrders = true
            };
            var data = new Data(client, algorithmSettings, persistence);

            // Act.
            data.Load();

            // Assert.
            Assert.Empty(data.History);
        }
    }
}