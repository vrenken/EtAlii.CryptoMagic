namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Linq;

    public class ApplicationContext
    {
        public static IClient LiveClient => _liveClient;
        private static IClient _liveClient;
        
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
                _liveClient = new Client(new ActionValidator());
                _liveClient.Start(binanceApiKey, binanceSecretKey);
                Symbols = _liveClient
                    .GetSymbols(ReferenceSymbol)
                    .ToArray();
                IsOperational = true;
            }
            else
            {
                _liveClient = null;
                Symbols = Array.Empty<SymbolDefinition>();
                IsOperational = false;
            }
        }
    }
}