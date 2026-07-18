using Sol.Common.Pagination;
using Sol.Core.Services.Jobs.Requests;
using Sol.Core.Services.Jobs.Responses;

namespace Sol.Core.Services.Jobs;

public interface IJobsService
{
    Task<PagedList<JobModel>> GetJobsAsync(GetJobsRequest request, CancellationToken cancellationToken);

    Task<JobModel> GetJobByIdAsync(string id, CancellationToken cancellationToken);

    Task<JobModel> CreateJobAsync(CreateJobRequest request, CancellationToken cancellationToken);

    Task<JobModel> UpdateJobAsync(UpdateJobRequest request, CancellationToken cancellationToken);
}