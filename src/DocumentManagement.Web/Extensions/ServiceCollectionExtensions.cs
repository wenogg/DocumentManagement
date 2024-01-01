using DocumentManagement.Core.Documents;
using DocumentManagement.Core.Files;
using DocumentManagement.Persistence.Files;
using Storage.Net;

namespace DocumentManagement.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = StorageFactory.Blobs.InMemory);

        return services
            .AddSingleton<IFileStorage, FileStorage>()
            .AddScoped<IDocumentService, DocumentService>();
    }
}