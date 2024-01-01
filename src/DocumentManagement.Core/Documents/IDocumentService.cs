using DocumentManagement.Core.Files;
using MediatR;

namespace DocumentManagement.Core.Documents;

public interface IDocumentService
{
    Task<Document> SaveDocumentAsync(string fileName, Stream data, string documentTypeId, CancellationToken cancellationToken = default);
}

public class DocumentService(
    IFileStorage fileStorage,
    IDocumentStore documentStore,
    TimeProvider systemClock,
    IPublisher mediator)
    : IDocumentService
{
    public async Task<Document> SaveDocumentAsync(string fileName, Stream data, string documentTypeId, CancellationToken cancellationToken = default)
    {
        // Persist the uploaded file.
        await fileStorage.Write(data, fileName, cancellationToken);

        // Create a document record.
        var document = new Document
        {
            Id = Guid.NewGuid().ToString("N"),
            Status = DocumentStatus.New,
            DocumentTypeId = documentTypeId,
            CreatedAt = systemClock.GetUtcNow().UtcDateTime,
            FileName = fileName
        };

        // Save the document.
        await documentStore.Save(document, cancellationToken);
        var newDocument = await documentStore.Get(document.Id, cancellationToken);

        // Publish a domain event.
        await mediator.Publish(new NewDocumentReceived(newDocument), cancellationToken);

        return document;
    }
}