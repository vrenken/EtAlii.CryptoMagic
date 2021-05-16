namespace EtAlii.BinanceMagic.Service.Trading.Simple
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.EntityFrameworkCore;

    public partial class SimpleEdit
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
                var simpleTrading = data.SimpleTradings.Single(t => t.Id == id);
                _model.Name = simpleTrading?.Name;
                _model.Id = simpleTrading?.Id ?? Guid.Empty;
            }

            _editContext = new EditContext(_model);
        }

        private void HandleValidSubmit()
        {
            var data = new DataContext();

            var trading = _model.Id == Guid.Empty
                ? new SimpleTrading()
                : data.SimpleTradings.Single(t => t.Id == _model.Id);

            data.Entry(trading).State = _model.Id == Guid.Empty
                ? EntityState.Added
                : EntityState.Modified;

            trading.Name = _model.Name;

            data.SaveChanges();
            
            _navigationManager.NavigateTo($"/simple/view/{trading.Id}");
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