using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Providers.Workflows;
using Elsa.Server.Hangfire.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net;

namespace DocumentManagement.Workflows.Extensions;

public static class ServiceCollectionExtensions
{
    // public static IServiceCollection AddWorkflowServices(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    // {
    //     return services.AddElsa(configureDb);
    // }

    public static IServiceCollection AddElsa(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> configureDb)
    {
        var elsaSection = config.GetSection("Elsa");
        services
            .AddElsa(elsa => elsa
                // Use EF Core's SQLite provider to store workflow instances and bookmarks.
                .UseEntityFrameworkPersistence(configureDb)
                // Ue Console activities for testing & demo purposes.
                .AddConsoleActivities()
                // Use Hangfire to dispatch workflows from.
                .UseHangfireDispatchers()
                .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                .AddEmailActivities(elsaSection.GetSection("Smtp").Bind)
            );

        // Configure Storage for BlobStorageWorkflowProvider with a directory on disk from where to load workflow definition JSON files from the local "Workflows" folder.
        var currentAssemblyPath = Path.GetDirectoryName(typeof(ServiceCollectionExtensions).Assembly.Location)!;

        services.Configure<BlobStorageWorkflowProviderOptions>(options =>
            options.BlobStorageFactory = () =>
                StorageFactory.Blobs.DirectoryFiles(Path.Combine(currentAssemblyPath, "WorkflowDefinitions")));

        return services;
    }
}