namespace DocumentManagement.Core.Documents;

public interface IDocumentStore
{
    Task Save(Document entity, CancellationToken cancellationToken = default);
    Task<Document?> Get(string id, CancellationToken cancellationToken = default);
}