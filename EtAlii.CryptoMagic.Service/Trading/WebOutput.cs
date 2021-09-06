namespace EtAlii.CryptoMagic.Service
{
    using System.Text;

    public class WebOutput : IOutput
    {
        public string Result { get; private set; } = string.Empty;

        private readonly StringBuilder _builder = new ();
        
        public void Write(string text)
        {
            _builder.Append(text);
            Result = _builder.ToString();
        }

        public void WriteLine(string line)
        {
            _builder.AppendLine(line + "<br/>");
            Result = _builder.ToString();
        }

        public void WriteLineFormatted(string line, params object[] parameters)
        {
            line = string.Format(line, parameters);
            _builder.AppendLine(line + "<br/>");
            Result = _builder.ToString();
        }

        public void WriteLinePositive(string line)
        {
            _builder.AppendLine("<font color='green'>" + line + "</font><br/>");
            Result = _builder.ToString();
        }
        public void WriteLineNegative(string line)
        {
            _builder.AppendLine("<font color='red'>" + line + "</font><br/>");
            Result = _builder.ToString();
        }
    }
}