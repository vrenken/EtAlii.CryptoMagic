namespace EtAlii.BinanceMagic
{
    public record ProgramSettings
    {
        public bool IsTest { get; init; } = false;
        public bool PlaceTestOrders { get; init; } = false;
        public string ApiKey  { get; init; } = "tLLXzKjw2rmhbJeGZlGSEwWUzrKesTzlPNZphZLueMaaPzzaO7A7LYEszaC6QE4f";
        public string SecretKey  { get; init; } = "10Mr5QAxuEAcXGdtl10pKqHBx5HrsJcd5fdNbXN08gpjL8tFh7Vml2pEm2TVFtmd";
    }
}