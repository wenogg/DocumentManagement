namespace DocumentManagement.Core.Files;

public interface IFileStorage
{
    Task Write(Stream data, string fileName, CancellationToken cancellationToken = default);
    Task<Stream> Read(string fileName, CancellationToken cancellationToken = default);
}