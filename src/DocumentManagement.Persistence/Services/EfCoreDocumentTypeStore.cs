using DocumentManagement.Core.Documents;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Persistence.Services;

public class EfCoreDocumentTypeStore(IDbContextFactory<DocumentDbContext> dbContextFactory) : IDocumentTypeStore
{
    public async Task<IEnumerable<DocumentType>> List(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.DocumentTypes.ToListAsync(cancellationToken);
    }

    public async Task<DocumentType?> Get(string id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.DocumentTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}