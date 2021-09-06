namespace EtAlii.CryptoMagic
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SettingEntityTypeConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.HasIndex(e => e.Key).IsUnique();
            builder.Property(e => e.Key).IsRequired();
            builder.Property(e => e.Value).IsRequired();
        }
    }
}