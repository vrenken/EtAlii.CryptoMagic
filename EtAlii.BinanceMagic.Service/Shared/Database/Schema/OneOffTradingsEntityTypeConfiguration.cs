namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OneOffTradingsEntityTypeConfiguration : IEntityTypeConfiguration<OneOffTrading>
    {
        public void Configure(EntityTypeBuilder<OneOffTrading> builder)
        {
        }
    }
}