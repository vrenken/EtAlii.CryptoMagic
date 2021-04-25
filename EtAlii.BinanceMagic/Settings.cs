namespace EtAlii.BinanceMagic
{
    using System;

    public record Settings
    {
        public bool IsTest { get; init; } = false;
        public bool PlaceTestOrders { get; init; } = false;
        public string TransactionsFile { get; init; }  = "Transactions.txt";
        public string TrendsFile { get; init; }  = "Trends.txt";
        
        public string[] AllowedCoins { get; init; }  = { "BTC", "BNB" }; // "ETH"
        
        public string ApiKey  { get; init; } = "tLLXzKjw2rmhbJeGZlGSEwWUzrKesTzlPNZphZLueMaaPzzaO7A7LYEszaC6QE4f";
        public string SecretKey  { get; init; } = "10Mr5QAxuEAcXGdtl10pKqHBx5HrsJcd5fdNbXN08gpjL8tFh7Vml2pEm2TVFtmd";

        public string ReferenceCoin  { get; init; } = "BUSD";
        
        public decimal MinimalIncrease  { get; init; } = 0.05m; // in %
        public decimal InitialPurchaseMinimalFactor { get; init; } = 10.0m; // in %
        public decimal MinimalTargetProfit  { get; init; } = 11m; // in BUSD.
        public decimal MaxQuantityToTrade  { get; init; } = 1.0m; // in %.
        public decimal NotionalMinCorrection  { get; init; } = 1.05m; // in %.

        public TimeSpan SampleInterval  { get; init; } = TimeSpan.FromMinutes(1);

    }
}