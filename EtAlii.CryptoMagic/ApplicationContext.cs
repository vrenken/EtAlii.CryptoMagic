namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;

    public class ApplicationContext
    {
        public string ReferenceSymbol { get; private set; }
        
        public SymbolDefinition[] Symbols { get; private set; }
        public bool IsOperational { get; private set; }

        public void Initialize()
        {
            using var data = new DataContext();

            var binanceApiKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.BinanceApiKey)?.Value;
            var binanceSecretKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.BinanceSecretKey)?.Value;
            ReferenceSymbol = data.Settings.SingleOrDefault(s => s.Key == SettingKey.ReferenceSymbol)?.Value;

            if (!string.IsNullOrWhiteSpace(binanceApiKey) && !string.IsNullOrWhiteSpace(binanceSecretKey))
            {
                var liveClient = new Client(new ActionValidator());
                var task = liveClient.Start(binanceApiKey, binanceSecretKey);
                task.Wait();
                
                Symbols = liveClient
                    .GetSymbols(ReferenceSymbol)
                    .ToArray();
                IsOperational = true;
            }
            else
            {
                Symbols = Array.Empty<SymbolDefinition>();
                IsOperational = false;
            }
        }
    }
}