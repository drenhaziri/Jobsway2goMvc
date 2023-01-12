using Jobsway2goMvc.Interfaces;

namespace Jobsway2goMvc.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseSqlTableDependency<T>(this IApplicationBuilder applicationBuilder, string connectionString)
            where T : ISubscribeTableDependency
    {
        var serviceProvider = applicationBuilder.ApplicationServices;
        var service = serviceProvider.GetService<T>();
        service?.SubscribeTableDependency(connectionString);
    }
}
