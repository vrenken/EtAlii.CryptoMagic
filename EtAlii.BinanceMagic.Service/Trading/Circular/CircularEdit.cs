namespace EtAlii.BinanceMagic.Service.Trading.Circular
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Blazorise;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    public partial class CircularEdit
    {
        private Validations _validations;
        
        private readonly Model _model = new();
        private EditContext _editContext;

        [Parameter] public string Id { get; set; }
        
        protected override void OnInitialized()
        {
            if (Id != null)
            {
                var id = Guid.Parse(Id);
                var data = new DataContext();
                var circularTrading = data.CircularTradings.Single(t => t.Id == id);
                _model.Name = circularTrading?.Name;
                _model.Id = circularTrading?.Id ?? Guid.Empty;
            }

            _editContext = new EditContext(_model);
        }

        private void Submit()
        {
            if (_validations.ValidateAll())
            {
                var data = new DataContext();

                var trading = _model.Id == Guid.Empty
                    ? new CircularTrading()
                    : data.CircularTradings.Single(t => t.Id == _model.Id);

                trading.Name = _model.Name;
            
                _algorithmManager.Update(trading);
            
                _navigationManager.NavigateTo($"/circular/view/{trading.Id}");
            }
        }

        private class Model
        {
            public Guid Id { get; set; }

            [Required, Display(Name = "Name")] public string Name { get; set; }

            [Required, Display(Name = "Connectivity")]
            public string Connectivity { get; set; } = "BackTest";
        }
    }
}