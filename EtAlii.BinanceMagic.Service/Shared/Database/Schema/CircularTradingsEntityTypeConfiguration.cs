namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CircularTradingsEntityTypeConfiguration : IEntityTypeConfiguration<CircularTrading>
    {
        public void Configure(EntityTypeBuilder<CircularTrading> builder)
        {
            builder
                .Property(e => e.Connectivity)
                .IsRequired();

            builder
                .Property(e => e.FirstSymbol)
                .IsRequired();
            builder
                .Property(e => e.SecondSymbol)
                .IsRequired();
            
            builder.HasMany(e => e.Snapshots)
                .WithOne(s => s.Trading)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}