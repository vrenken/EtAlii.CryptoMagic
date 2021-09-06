namespace EtAlii.CryptoMagic
{
    public interface IOutput
    {
        void Write(string text);
        void WriteLine(string line);
        void WriteLineFormatted(string line, params object[] parameters);
        void WriteLinePositive(string line);
        void WriteLineNegative(string line);
    }
}