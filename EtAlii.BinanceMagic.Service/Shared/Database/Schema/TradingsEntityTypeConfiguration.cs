namespace EtAlii.BinanceMagic.Service.Shared
{
    using EtAlii.BinanceMagic.Service.Trading;
    using EtAlii.BinanceMagic.Service.Trading.Circular;
    using EtAlii.BinanceMagic.Service.Trading.Experimental;
    using EtAlii.BinanceMagic.Service.Trading.Simple;
    using EtAlii.BinanceMagic.Service.Trading.Surfing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class TradingsEntityTypeConfiguration : IEntityTypeConfiguration<TradingBase>
    {
        public void Configure(EntityTypeBuilder<TradingBase> builder)
        {
            builder.ToTable("Tradings");
            
            builder
                .Property(e => e.Name)
                .IsRequired();
            
            builder
                .HasDiscriminator<string>("trading_type")
                .HasValue<CircularTrading>("circular")
                .HasValue<SurfingTrading>("surfing")
                .HasValue<SimpleTrading>("simple")
                .HasValue<ExperimentalTrading>("experimental");
        }
    }
}