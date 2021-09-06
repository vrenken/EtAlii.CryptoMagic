namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public static class ModelBuilderApplyConfigurationExtension
    {
        /// <summary>
        /// Use this extension method to apply one generic IEntityTypeConfiguration implementation and
        /// apply it to multiple entities. This is especially useful for the base Entity class (and corresponding Id) we use. 
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="configurationType"></param>
        /// <param name="entityType"></param>
        /// <typeparam name="T"></typeparam>
        public static void ApplyConfiguration<T>(this ModelBuilder modelBuilder, Type configurationType, Type entityType)
        {
            if (typeof(T).IsAssignableFrom(entityType))
            {
                // Build IEntityTypeConfiguration type with generic type parameter
                var configurationGenericType = configurationType.MakeGenericType(entityType);
                // Create an instance of the IEntityTypeConfiguration implementation
                var configuration = Activator.CreateInstance(configurationGenericType);
                // Get the ApplyConfiguration method of ModelBuilder via reflection
                var applyEntityConfigurationMethod = typeof(ModelBuilder)
                    .GetMethods()
                    .Single(e => e.Name == nameof(ModelBuilder.ApplyConfiguration)
                                 && e.ContainsGenericParameters
                                 && e.GetParameters().SingleOrDefault()?.ParameterType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));
                // Create a generic ApplyConfiguration method with our entity type
                var target = applyEntityConfigurationMethod.MakeGenericMethod(entityType);
                // Invoke ApplyConfiguration, passing our IEntityTypeConfiguration instance
                target.Invoke(modelBuilder, new[] { configuration });
            }
        }
    }
}