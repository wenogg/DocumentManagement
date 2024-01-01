using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DocumentManagement.Persistence.HostedServices;

public class RunMigrations(IDbContextFactory<DocumentDbContext> dbContextFactoryFactory)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactoryFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.Database.MigrateAsync(cancellationToken);
        await dbContext.DisposeAsync().ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}