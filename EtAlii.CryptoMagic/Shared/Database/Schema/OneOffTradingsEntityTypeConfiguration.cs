namespace EtAlii.CryptoMagic
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OneOffTradingsEntityTypeConfiguration : IEntityTypeConfiguration<OneOffTrading>
    {
        public void Configure(EntityTypeBuilder<OneOffTrading> builder)
        {
            // Use the builder to configure any rules that relate to the OneOffTrading entity.
        }
    }
}