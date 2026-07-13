using Sol.Data.Context;
using Sol.Data.Repositories.Jobs;

namespace Sol.Data.Services;

/// <summary>
/// Context within which read/write operations may be performed. All repositories accessed within a UnitOfWork will
/// share a database context. 
/// </summary>
/// <param name="factory">The database context factory.</param>
/// <remarks>
/// As this service is responsible for managing the database contexts that it initializes, we implement the disposable
/// interfaces to ensure those contexts are disposed with the request scope.
/// </remarks>
public class UnitOfWork(IApiDbContextFactory factory) : IUnitOfWork, IDisposable, IAsyncDisposable
{
    private ApiDbContext? _context;
    private IJobsRepository? _jobs;

    /// <summary>The database context for this unit of work.</summary>
    private ApiDbContext Context => _context ??= factory.CreateDbContext(ContextType.ReadWrite);

    /// <inheritdoc />
    public IJobsRepository Jobs => _jobs ??= new JobsRepository(Context);

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await Context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_context != null)
            await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}