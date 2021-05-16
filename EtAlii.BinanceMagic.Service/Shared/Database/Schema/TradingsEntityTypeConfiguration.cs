namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TradingsEntityTypeConfiguration : IEntityTypeConfiguration<TradingBase>
    {
        public void Configure(EntityTypeBuilder<TradingBase> builder)
        {
            builder.ToTable("Trading");
            
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