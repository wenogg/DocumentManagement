namespace DocumentManagement.Core.Documents;

public interface IDocumentTypeStore
{
    Task<IEnumerable<DocumentType>> List(CancellationToken cancellationToken = default);
    Task<DocumentType?> Get(string id, CancellationToken cancellationToken = default);
}