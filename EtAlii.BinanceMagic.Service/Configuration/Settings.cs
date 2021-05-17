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
            var data = new DataContext();
            _model.ApiKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.ApiKey)?.Value;
            _model.SecretKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.SecretKey)?.Value;
            _model.ReferenceSymbol = data.Settings.SingleOrDefault(s => s.Key == SettingKey.ReferenceSymbol)?.Value;
                
            _editContext = new EditContext(_model);
        }

        private void Submit()
        {
            if (_validations.ValidateAll())
            {
                var data = new DataContext();

                UpdateSetting(data, SettingKey.ApiKey, _model.ApiKey);
                UpdateSetting(data, SettingKey.SecretKey, _model.SecretKey);
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
            [Required, Display(Name = "API key")] public string ApiKey { get; set; }

            [Required, Display(Name = "Secret key")] public string SecretKey { get; set; }
            [Required, Display(Name = "Reference symbol")] public string ReferenceSymbol { get; set; }
        }
    }
}