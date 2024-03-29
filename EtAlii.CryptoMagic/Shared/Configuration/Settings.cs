﻿namespace EtAlii.CryptoMagic
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazorise;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.EntityFrameworkCore;

    public partial class Settings
    {
        private readonly Model _model = new();
        private EditContext _editContext;
        private Validations _validations;

        [Inject] NavigationManager NavigationManager { get; init; }

        protected override void OnInitialized()
        {
            using var data = new DataContext();
            _model.BinanceApiKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.BinanceApiKey)?.Value;
            _model.BinanceSecretKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.BinanceSecretKey)?.Value;
            _model.ReferenceSymbol = data.Settings.SingleOrDefault(s => s.Key == SettingKey.ReferenceSymbol)?.Value;
                
            _editContext = new EditContext(_model);
        }

        private async Task Submit()
        {
            if (await _validations.ValidateAll())
            {
                using var data = new DataContext();

                UpdateSetting(data, SettingKey.BinanceApiKey, _model.BinanceApiKey);
                UpdateSetting(data, SettingKey.BinanceSecretKey, _model.BinanceSecretKey);
                UpdateSetting(data, SettingKey.ReferenceSymbol, _model.ReferenceSymbol);

                data.SaveChanges();
                
                _applicationContext.Initialize();
                
                NavigationManager.NavigateTo("/");
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