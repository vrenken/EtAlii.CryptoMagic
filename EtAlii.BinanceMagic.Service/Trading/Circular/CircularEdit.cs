namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;

    public partial class CircularEdit 
    {
        private decimal TargetIncreaseInPercentage { get => Model.TargetIncrease * 100; set => Model.TargetIncrease = value / 100m; }
        private decimal InitialSellFactorInPercentage { get => Model.InitialSellFactor * 100; set => Model.InitialSellFactor = value / 100m; }
        private decimal MaxQuantityToTradeInPercentage { get => Model.MaxQuantityToTrade * 100; set => Model.MaxQuantityToTrade = value / 100m; }
        private decimal NotionalMinCorrectionInPercentage { get => Model.NotionalMinCorrection * 100; set => Model.NotionalMinCorrection = value / 100m; }
        
            
        protected override CircularTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.CircularTradings.Single(t => t.Id == id);
        }

        protected override CircularTrading CreateTrading()
        {
            using var data = new DataContext();
            return new CircularTrading
            {
                ReferenceSymbol = data.Settings.Single(s => s.Key == SettingKey.ReferenceSymbol).Value
            };
        }

        protected override string GetNavigationUrl(Guid id) => $"/circular/view/{Model.Id}";
    }
}