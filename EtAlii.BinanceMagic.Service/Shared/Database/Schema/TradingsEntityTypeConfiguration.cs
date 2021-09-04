namespace EtAlii.BinanceMagic.Service
{
    using EtAlii.BinanceMagic.Service.Surfing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TradingsEntityTypeConfiguration : IEntityTypeConfiguration<TradingBase>
    {
        public void Configure(EntityTypeBuilder<TradingBase> builder)
        {
            builder.ToTable("Tradings");
            
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.ReferenceSymbol).IsRequired();
            
            builder
                .HasDiscriminator<string>("trading_type")
                .HasValue<CircularTrading>("circular")
                .HasValue<SurfingTrading>("surfing")
                .HasValue<SimpleTrading>("simple")
                .HasValue<OneOffTrading>("one-off")
                .HasValue<EdgeTrading>("edge")
                .HasValue<ExperimentalTrading>("experimental");
        }
    }
}