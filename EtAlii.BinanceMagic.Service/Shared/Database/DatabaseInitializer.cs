namespace EtAlii.BinanceMagic.Service.Shared
{
    using System.Linq;

    public class DatabaseInitializer
    {
        public void InitializeWhenNeeded()
        {
            using var data = new DataContext();
            data.Database.EnsureCreated();
            
            if (!data.Settings.Any())
            {
                
            }
        }
    }
}