namespace EtAlii.BinanceMagic
{
    public interface IProgram
    {
        void HandleFail(string message);
        void HandleFinish(string message);
    }
}