namespace EtAlii.BinanceMagic.Circular
{
    public interface ICircularAlgorithm
    {
        bool TransactionIsWorthIt(Situation situation, out SellAction sellAction, out BuyAction buyAction);
        void ToInitialConversionActions(Situation situation, out SellAction sellAction, out BuyAction buyAction);
    }
}