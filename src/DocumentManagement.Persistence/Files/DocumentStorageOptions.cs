using Storage.Net;
using Storage.Net.Blobs;

namespace DocumentManagement.Persistence.Files;

public class DocumentStorageOptions
{
    public Func<IBlobStorage> BlobStorageFactory { get; set; } = StorageFactory.Blobs.InMemory;
}
