using DocumentManagement.Core.Documents;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Persistence.Services;

public class EfCoreDocumentStore(IDbContextFactory<DocumentDbContext> dbContextFactory) : IDocumentStore
{
    public async Task Save(Document entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existingDocument = await dbContext.Documents.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

        if(existingDocument == null)
            await dbContext.Documents.AddAsync(entity, cancellationToken);
        else
            dbContext.Entry(existingDocument).CurrentValues.SetValues(entity);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Document?> Get(string id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Documents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}