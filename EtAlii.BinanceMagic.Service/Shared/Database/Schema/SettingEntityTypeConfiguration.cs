namespace EtAlii.BinanceMagic.Service.Shared
{
    using EtAlii.BinanceMagic.Service.Configuration;
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