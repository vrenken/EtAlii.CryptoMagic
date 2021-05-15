namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SettingEntityTypeConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.Property(settings => settings.Key).IsRequired();
            builder.Property(settings => settings.Value).IsRequired();
        }
    }
}