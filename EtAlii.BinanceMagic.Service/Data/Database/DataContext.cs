namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.EntityFrameworkCore;
    using Serilog;
    using Serilog.Extensions.Logging;

    public class DataContext : DbContext
    {
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Experiment> Experiments { get; set; }
        
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
            modelBuilder.ApplyConfiguration(new ExperimentEntityTypeConfiguration());
        }
    }
}