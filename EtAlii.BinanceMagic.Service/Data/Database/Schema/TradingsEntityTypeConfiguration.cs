namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TradingsEntityTypeConfiguration : IEntityTypeConfiguration<Trading>
    {
        public void Configure(EntityTypeBuilder<Trading> builder)
        {
            builder.Property(settings => settings.Name).IsRequired();
            
            builder
                .HasDiscriminator<string>("trading_type")
                .HasValue<CircularTrading>("circular")
                .HasValue<SurfingTrading>("surfing")
                .HasValue<SurfingTrading>("simple")
                .HasValue<ExperimentalTrading>("experimental");
        }
    }
}