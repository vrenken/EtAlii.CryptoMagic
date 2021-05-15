namespace EtAlii.BinanceMagic.Service.Pages
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.EntityFrameworkCore;

    public partial class Settings
    {
        private Model _model = new();
        private EditContext _editContext;

        protected override void OnInitialized()
        {
            var data = new DataContext();
            _model.ApiKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.ApiKey)?.Value;
            _model.SecretKey = data.Settings.SingleOrDefault(s => s.Key == SettingKey.SecretKey)?.Value;
                
            _editContext = new EditContext(_model);
        }

        private void HandleValidSubmit()
        {
            //var modelJson = JsonSerializer.Serialize(_model, new JsonSerializerOptions { WriteIndented = true });
            //_jsRuntime.InvokeVoidAsync("alert", $"SUCCESS!! :-)\n\n{modelJson}");
            
            var data = new DataContext();
            var apiKeySetting = data.Settings.SingleOrDefault(s => s.Key == SettingKey.ApiKey);
            if (apiKeySetting == null)
            {
                apiKeySetting = new Setting {Key = SettingKey.ApiKey};
                data.Entry(apiKeySetting).State = EntityState.Added;
            }
            else
            {
                data.Entry(apiKeySetting).State = EntityState.Modified;
            }
            apiKeySetting.Value = _model.ApiKey;
            
            var secretKeySetting = data.Settings.AsTracking().SingleOrDefault(s => s.Key == SettingKey.SecretKey);
            if (secretKeySetting == null)
            {
                secretKeySetting = new Setting {Key = SettingKey.SecretKey};
                data.Entry(secretKeySetting).State = EntityState.Added;
            }
            else
            {
                data.Entry(secretKeySetting).State = EntityState.Modified;
            }
            secretKeySetting.Value = _model.SecretKey;
            
            data.SaveChanges();
        }

        private void HandleReset()
        {
            _model = new Model();
            _editContext = new EditContext(_model);
        }

        private class Model
        {
            [Required]
            [Display(Name = "API key")]
            public string ApiKey { get; set; }

            [Required]
            [Display(Name = "Secret key")]
            public string SecretKey { get; set; }
        }
    }
}