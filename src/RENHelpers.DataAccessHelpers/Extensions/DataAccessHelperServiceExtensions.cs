using Microsoft.Extensions.DependencyInjection;
using RENHelpers.DataAccessHelpers.CacheHelpers;
using RENHelpers.DataAccessHelpers.DatabaseHelpers;

namespace RENHelpers.DataAccessHelpers.Extensions;

public static class DataAccessHelperServiceExtensions
{
    public static IServiceCollection RegisterRENDatabaseAccessHelpers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRENUnitOfWork<>), typeof(RENUnitOfWork<>));

        return services;
    }

    public static IServiceCollection RegisterRENCacheAccessHelpers<T>(this IServiceCollection services) where T : IRENCacheService
    {
        services.AddScoped(typeof(IRENCacheService), typeof(T));

        return services;
    }
}