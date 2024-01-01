using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Server.Hangfire.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentManagement.Workflows.Extensions;

public static class ServiceCollectionExtensions
{
    // public static IServiceCollection AddWorkflowServices(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    // {
    //     return services.AddElsa(configureDb);
    // }

    private static IServiceCollection AddElsa(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services
            .AddElsa(elsa => elsa
                // Use EF Core's SQLite provider to store workflow instances and bookmarks.
                .UseEntityFrameworkPersistence(configureDb)

                // Ue Console activities for testing & demo purposes.
                .AddConsoleActivities()
                // Use Hangfire to dispatch workflows from.
                .UseHangfireDispatchers()
                // Configure Email activities.
                .AddEmailActivities()
                // Configure HTTP activities.
                .AddHttpActivities()
            );

        return services;
    }
}