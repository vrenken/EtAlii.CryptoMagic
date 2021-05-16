namespace EtAlii.BinanceMagic.Service.Trading.Circular
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.EntityFrameworkCore;

    public partial class CircularEdit
    {
        private Model _model = new();
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

        private void HandleValidSubmit()
        {
            var data = new DataContext();

            var trading = _model.Id == Guid.Empty
                ? new CircularTrading()
                : data.CircularTradings.Single(t => t.Id == _model.Id);

            data.Entry(trading).State = _model.Id == Guid.Empty
                ? EntityState.Added
                : EntityState.Modified;

            trading.Name = _model.Name;

            data.SaveChanges();
            
            _navigationManager.NavigateTo($"/circular/view/{trading.Id}");
        }

        private void HandleReset()
        {
            _model = new Model();
            _editContext = new EditContext(_model);
        }

        private class Model
        {
            public Guid Id { get; set; }

            [Required] [Display(Name = "Name")] public string Name { get; set; }
        }
    }
}