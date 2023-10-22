using Microsoft.Extensions.DependencyInjection;
using RENHelpers.DataAccessHelpers.EntityFrameworkAccess;

namespace RENHelpers.DataAccessHelpers.Extensions;

public static class DataAccessHelperServiceExtensions
{
    public static IServiceCollection RegisterRENDatabaseAccessHelpers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRENUnitOfWork<>), typeof(RENUnitOfWork<>));

        return services;
    }
}