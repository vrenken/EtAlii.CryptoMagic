namespace EtAlii.BinanceMagic.Service
{
    using System.Linq;

    public class ApplicationContext
    {
        public IClient LiveClient => _liveClient;
        private static IClient _liveClient;
        
        public void Initialize()
        {
            using var data = new DataContext();

            var binanceApiKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.BinanceApiKey)?.Value;
            var binanceSecretKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.BinanceSecretKey)?.Value;
            if (!string.IsNullOrWhiteSpace(binanceApiKey) && !string.IsNullOrWhiteSpace(binanceSecretKey))
            {
                _liveClient = new Client(new ActionValidator());
                _liveClient.Start(binanceApiKey, binanceSecretKey);
            }
            else
            {
                _liveClient = null;
            }
        }
    }
}