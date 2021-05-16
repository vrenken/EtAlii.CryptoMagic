namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : Entity
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder
                .HasIndex(entity => entity.Id)
                .IsUnique();

            builder
                .HasKey(entity => entity.Id);

            builder
                .Property(entity => entity.Id)
                .ValueGeneratedOnAdd();
        }
    }
}