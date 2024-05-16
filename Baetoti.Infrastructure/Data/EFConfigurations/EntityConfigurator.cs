using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.EFConfigurations
{
    public class EntityConfigurator
    {
        public static void Configure<Entity, Configuration>(ModelBuilder builder)
            where Entity : class
            where Configuration : new()
        {
            var entityTypeBuilder = builder.Entity<Entity>();
            var configuration = new Configuration();

            var configurationType = configuration.GetType();
            var method = configurationType.GetMethod("Configure");
            method.Invoke(configuration, new object[] { entityTypeBuilder });
        }

    }
}
