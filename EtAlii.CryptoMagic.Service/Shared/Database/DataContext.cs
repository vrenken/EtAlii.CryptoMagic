namespace EtAlii.CryptoMagic.Service
{
    using EtAlii.CryptoMagic.Service.Surfing;
    using Microsoft.EntityFrameworkCore;
    using Serilog;
    using Serilog.Extensions.Logging;

    public class DataContext : DbContext
    {
        public DbSet<Setting> Settings { get; init; }
        
        public DbSet<SimpleTrading> SimpleTradings { get; init; }
        public DbSet<CircularTrading> CircularTradings { get; init; }
        public DbSet<Snapshot> Snapshots { get; init; }
        public DbSet<SurfingTrading> SurfingTradings { get; init; }
        public DbSet<EdgeTrading> EdgeTradings { get; init; }
        public DbSet<ExperimentalTrading> ExperimentalTradings { get; init; }
        public DbSet<OneOffTrading> OneOffTradings { get; init; }
        
        public DbSet<CircularTransaction> CircularTransactions { get; init; }
        
        private readonly ILogger _logger = Log.ForContext<DataContext>();

        // The following configures EF to create a Sqlite database file as `C:\blogging.db`.
        // For Mac or Linux, change this to `/tmp/blogging.db` or any other absolute path.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(@"Data Source=database.db");
            
            var loggerFactory = new SerilogLoggerFactory(_logger);
            options.UseLoggerFactory(loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.ApplyConfiguration<Entity>(typeof(EntityTypeConfiguration<>), entityType.ClrType);
            }
            
            modelBuilder.ApplyConfiguration(new SettingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TradingsEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CircularTradingsEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EdgeTradingsEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OneOffTradingsEntityTypeConfiguration());
        }
    }
}