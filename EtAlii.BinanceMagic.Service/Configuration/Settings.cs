namespace EtAlii.BinanceMagic.Service.Configuration
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Blazorise;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.EntityFrameworkCore;

    public partial class Settings
    {
        private Model _model = new();
        private EditContext _editContext;
        private Validations _validations;

        protected override void OnInitialized()
        {
            using var data = new DataContext();
            _model.BinanceApiKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.BinanceApiKey)?.Value;
            _model.BinanceSecretKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.BinanceSecretKey)?.Value;
            _model.ReferenceSymbol = data.Settings.SingleOrDefault(s => s.Key == SettingKey.ReferenceSymbol)?.Value;
                
            _editContext = new EditContext(_model);
        }

        private void Submit()
        {
            if (_validations.ValidateAll())
            {
                using var data = new DataContext();

                UpdateSetting(data, SettingKey.BinanceApiKey, _model.BinanceApiKey);
                UpdateSetting(data, SettingKey.BinanceSecretKey, _model.BinanceSecretKey);
                UpdateSetting(data, SettingKey.ReferenceSymbol, _model.ReferenceSymbol);

                data.SaveChanges();
            }
        }

        private void UpdateSetting(DataContext data, string key, string value)
        {
            var setting = data.Settings.SingleOrDefault(s => s.Key == key);
            if (setting == null)
            {
                setting = new Setting {Key = key};
                data.Entry(setting).State = EntityState.Added;
            }
            else
            {
                data.Entry(setting).State = EntityState.Modified;
            }
            setting.Value = value;
        }

        private class Model
        {
            [Required, Display(Name = "Binance API key")] public string BinanceApiKey { get; set; }

            [Required, Display(Name = "Binance secret key")] public string BinanceSecretKey { get; set; }
            
            [Required, Display(Name = "Reference symbol")] public string ReferenceSymbol { get; set; }
        }
    }
}