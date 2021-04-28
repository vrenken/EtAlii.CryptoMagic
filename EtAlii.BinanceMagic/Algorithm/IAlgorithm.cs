namespace EtAlii.BinanceMagic
{
    public interface IAlgorithm
    {
        bool TransactionIsWorthIt(Situation situation, out SellAction sellAction, out BuyAction buyAction);
        void ToInitialConversionActions(Situation situation, out SellAction sellAction, out BuyAction buyAction);
    }
}