namespace EtAlii.BinanceMagic
{
    public interface IAlgorithm
    {
        bool TransactionIsWorthIt(Target target, Situation situation, out SellAction sellAction, out BuyAction buyAction);
        void ToInitialConversionActions(Target target, Situation situation, out SellAction sellAction, out BuyAction buyAction);
    }
}