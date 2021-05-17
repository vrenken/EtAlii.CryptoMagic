namespace EtAlii.BinanceMagic.Service.Shared
{
    using EtAlii.BinanceMagic.Service.Trading.Circular;
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
        }
    }
}