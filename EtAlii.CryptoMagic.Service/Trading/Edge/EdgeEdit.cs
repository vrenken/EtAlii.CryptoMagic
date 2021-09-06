namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Linq;

    public partial class EdgeEdit 
    {
        // private decimal QuantityFactorInPercentage { get => Model.QuantityFactor * 100; set => Model.QuantityFactor = value / 100m; }
        // private decimal TargetIncreaseInPercentage { get => Model.TargetIncrease * 100; set => Model.TargetIncrease = value / 100m; }
        // private decimal InitialSellFactorInPercentage { get => Model.InitialSellFactor * 100; set => Model.InitialSellFactor = value / 100m; }
        // private decimal MaxQuantityToTradeInPercentage { get => Model.MaxQuantityToTrade * 100; set => Model.MaxQuantityToTrade = value / 100m; }
        // private decimal NotionalMinCorrectionInPercentage { get => Model.NotionalMinCorrection * 100; set => Model.NotionalMinCorrection = value / 100m; }

        private string Symbol
        {
            get => Model.Symbol;
            set
            {
                Model.Symbol = value;
                if (Model.Name == null)
                {
                    Model.Name = $"{Model.Symbol} Edge trade";
                }
            }
        }
            
        protected override EdgeTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.EdgeTradings.Single(t => t.Id == id);
        }

        protected override string GetNavigationUrl(Guid id) => $"/edge/view/{Model.Id}";
    }
}