using Sol.Common.Pagination;
using Sol.Core.Exceptions;
using Sol.Core.Extensions;
using Sol.Core.Services.Jobs.Requests;
using Sol.Core.Services.Jobs.Responses;
using Sol.Core.Services.Validation;
using Sol.Data.Models;
using Sol.Data.Repositories.Jobs;
using Sol.Data.Services;
using Sol.Domain.Entities;
using Sol.Domain.Enums;

namespace Sol.Core.Services.Jobs;

public class JobsService(IReadOnlyJobsRepository repository, IUnitOfWork unitOfWork, IValidationService validator)
    : IJobsService
{
    public async Task<PagedList<JobModel>> GetJobsAsync(GetJobsRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateQueryAsync(request, cancellationToken);

        JobTypeEnum? type = request.Type is not null ? Enum.Parse<JobTypeEnum>(request.Type, true) : null;
        JobStatusEnum? status = request.Status is not null ? Enum.Parse<JobStatusEnum>(request.Status, true) : null;
        request.Cursor.TryDecodeCursor(out var cursor);

        var jobs = await repository.GetAsync(
            type,
            status,
            request.ErrorCode,
            new DateFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo),
            new PaginationFilter(cursor, request.PageSize),
            cancellationToken);

        return jobs.ToList().ToPagedList(JobModel.FromEntity);
    }

    public async Task<JobModel> GetJobByIdAsync(string id, CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(id, cancellationToken) is { } job
            ? JobModel.FromEntity(job)
            : throw new NotFoundException(typeof(Job), id);

    public async Task<JobModel> CreateJobAsync(CreateJobRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateCommandAsync(request, cancellationToken);

        if (await unitOfWork.Jobs.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken) is { } extant)
            return JobModel.FromEntity(extant);

        var type = Enum.Parse<JobTypeEnum>(request.Type, true);
        var job = new Job(request.IdempotencyKey, type, request.Parameters);

        var created = await unitOfWork.Jobs.CreateAsync(job, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return JobModel.FromEntity(created);
    }

    public async Task<JobModel> UpdateJobAsync(UpdateJobRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateCommandAsync(request, cancellationToken);

        var job = await unitOfWork.Jobs.GetByIdAsync(request.Id, cancellationToken) ??
                  throw new NotFoundException(typeof(Job), request.Id);

        ConcurrencyTokenMismatchException.ThrowIfMismatch(request.ConcurrencyToken, job.ConcurrencyToken);

        var status = Enum.Parse<JobStatusEnum>(request.Status, true);
        job.Update(status, request.ErrorCode, request.ErrorMessage);

        if (!job.IsDirty)
            return JobModel.FromEntity(job);

        await unitOfWork.Jobs.UpdateAsync(job, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return JobModel.FromEntity(job);
    }
}