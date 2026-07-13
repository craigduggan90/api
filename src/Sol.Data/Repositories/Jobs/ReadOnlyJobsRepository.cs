using Microsoft.EntityFrameworkCore;
using Sol.Data.Context;
using Sol.Data.Filters;
using Sol.Data.Models;
using Sol.Domain.Entities;
using Sol.Domain.Enums;

namespace Sol.Data.Repositories.Jobs;

/// <summary>A read-only repository containing instances of <see cref="Job"/>.</summary>
public class ReadOnlyJobsRepository(ApiDbContext context) : RepositoryBase(context), IReadOnlyJobsRepository
{
    /// <inheritdoc />
    public async Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken)
        => await Context.Jobs
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<Job?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken)
        => await Context.Jobs
            .SingleOrDefaultAsync(entity => entity.IdempotencyKey == idempotencyKey, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Job>> GetAsync(
        JobTypeEnum? type = null, 
        JobStatusEnum? status = null, 
        string? errorCode = null,
        DateTime? lastEventFrom = null, 
        DateTime? lastEventTo = null, 
        DateFilter? dateFilter = null,
        PaginationFilter? pagination = null, 
        CancellationToken cancellationToken = default)
        => await Context.Jobs
            .ApplyTypeFilter(type)
            .ApplyStatusFilter(status)
            .ApplyErrorCodeFilter(errorCode)
            .ApplyLastEventTimeFromFilter(lastEventFrom)
            .ApplyLastEventTimeToFilter(lastEventTo)
            .ApplyCreatedFromFilter(dateFilter?.CreatedFrom)
            .ApplyCreatedToFilter(dateFilter?.CreatedTo)
            .ApplyModifiedFromFilter(dateFilter?.ModifiedFrom)
            .ApplyModifiedToFilter(dateFilter?.ModifiedTo)
            .ApplyCursor(pagination?.Cursor)
            .ApplyPagination(pagination?.PageSize ?? Constants.DefaultPageSize)
            .ToListAsync(cancellationToken);
}