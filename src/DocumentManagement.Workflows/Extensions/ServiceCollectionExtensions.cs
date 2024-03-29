﻿using DocumentManagement.Workflows.Activities;
using DocumentManagement.Workflows.Handlers;
using DocumentManagement.Workflows.Scripting.JavaScript;
using Elsa;
using Elsa.Activities.UserTask.Extensions;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Providers.Workflows;
using Elsa.Server.Hangfire.Extensions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net;

namespace DocumentManagement.Workflows.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowServices(this IServiceCollection services)
    {
        services.AddNotificationHandlersFrom<StartDocumentWorkflows>();

        // Register custom type definition provider for JS intellisense.
        services.AddJavaScriptTypeDefinitionProvider<CustomTypeDefinitionProvider>();

        services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
        return services;
    }

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
                .AddActivitiesFrom<GetDocument>()
                .AddUserTaskActivities()
            );


        // Configure Storage for BlobStorageWorkflowProvider with a directory on disk from where to load workflow definition JSON files from the local "Workflows" folder.
        var currentAssemblyPath = Path.GetDirectoryName(typeof(ServiceCollectionExtensions).Assembly.Location)!;

        services.Configure<BlobStorageWorkflowProviderOptions>(options =>
            options.BlobStorageFactory = () =>
                StorageFactory.Blobs.DirectoryFiles(Path.Combine(currentAssemblyPath, "WorkflowDefinitions")));

        return services;
    }
}