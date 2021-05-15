namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ExperimentEntityTypeConfiguration : IEntityTypeConfiguration<Experiment>
    {
        public void Configure(EntityTypeBuilder<Experiment> builder)
        {
            builder.Property(settings => settings.Name).IsRequired();
        }
    }
}