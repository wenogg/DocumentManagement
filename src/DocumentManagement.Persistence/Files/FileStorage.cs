using DocumentManagement.Core.Files;
using Microsoft.Extensions.Options;
using Storage.Net.Blobs;

namespace DocumentManagement.Persistence.Files;

public class FileStorage(IOptions<DocumentStorageOptions> options) : IFileStorage
{
    private readonly IBlobStorage _blobStorage = options.Value.BlobStorageFactory();

    public Task Write(Stream data, string fileName, CancellationToken cancellationToken = default) =>
        _blobStorage.WriteAsync(fileName, data, false, cancellationToken);

    public Task<Stream> Read(string fileName, CancellationToken cancellationToken = default) =>
        _blobStorage.OpenReadAsync(fileName, cancellationToken);
}