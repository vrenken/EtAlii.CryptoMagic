namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EdgeTradingsEntityTypeConfiguration : IEntityTypeConfiguration<EdgeTrading>
    {
        public void Configure(EntityTypeBuilder<EdgeTrading> builder)
        {
            builder.Property(e => e.Symbol).IsRequired();
            builder.Property(e => e.InitialPurchase).IsRequired();
            
            builder.HasMany(e => e.Transactions)
                .WithOne(s => s.Trading)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}