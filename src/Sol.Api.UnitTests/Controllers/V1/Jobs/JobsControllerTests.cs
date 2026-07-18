using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sol.Api.Contracts.V1.RequestModels;
using Sol.Api.Contracts.V1.ResponseModels;
using Sol.Api.Controllers.V1.Jobs;
using Sol.Common;
using Sol.Common.Extensions;
using Sol.Common.Pagination;
using Sol.Common.Providers.Identifiers;
using Sol.Common.Providers.Temporal;
using Sol.Core.Services.Jobs;
using Sol.Core.Services.Jobs.Requests;
using Sol.Core.Services.Jobs.Responses;
using Sol.Domain.Enums;
using System.Text.Json;

namespace Sol.Api.UnitTests.Controllers.V1.Jobs;

/// <summary>
/// Controllers are really tested through integration tests, these really just serve to ensure that we're injecting
/// values into mappers correctly.
/// </summary>
public static class JobsControllerTests
{
    public abstract class JobControllerTestsBase
    {
        protected readonly IJobsService JobsService = Substitute.For<IJobsService>();

        private JobsController? _sut;

        protected JobsController GetOrCreateSut() =>
            _sut ??= new JobsController(JobsService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

        protected JobModel GetJobModel(
            string? id = null,
            string? idempotencyKey = null,
            string? concurrencyToken = null,
            string? type = null,
            string? status = null,
            object? parameters = null,
            string? errorCode = null,
            string? errorMessage = null
            )
        {
            return new JobModel(
                id ?? IdentifierProvider.Generate,
                idempotencyKey ?? Guid.NewGuid().ToString("N"),
                concurrencyToken ?? Guid.NewGuid().ToString("N").GetMd5Digest(),
                type ?? nameof(JobTypeEnum.ArchiveProjectJob),
                status ?? nameof(JobStatusEnum.Pending),
                parameters?.Serialize(new JsonSerializerOptions { PropertyNamingPolicy = null }),
                errorCode,
                errorMessage,
                DateTimeOffsetProvider.Now.UtcDateTime,
                DateTimeOffsetProvider.Now.UtcDateTime);
        }

        protected static void AssertResultValue<TResult, TValue>(IActionResult result, TValue expected)
            where TResult : ObjectResult
        {
            var objectResult = Assert.IsType<TResult>(result);
            var actual = Assert.IsType<TValue>(objectResult.Value);
            Assert.Equivalent(expected, actual);
        }

        protected void AssertEtagSet(string expected)
        {
            GetOrCreateSut().Response.Headers.TryGetValue(Constants.ETagHeaderKey, out var actual);
            Assert.Equal(expected, Assert.Single(actual)?.Trim('"'));
        }
    }

    public class GetJobs : JobControllerTestsBase
    {
        [Fact]
        public async Task ShouldReturnOk_WhenSuccess()
        {
            var request = new GetJobsRequestModel();
            var serviceResponse = new PagedList<JobModel>([GetJobModel(), GetJobModel(), GetJobModel()], "a cursor", 3);

            JobsService.GetJobsAsync(Arg.Any<GetJobsRequest>(), Arg.Any<CancellationToken>())
                .Returns(serviceResponse);

            var expected = serviceResponse.Map<JobModel, JobResponseModel>(x => x.ToJobResponseModel());

            var sut = GetOrCreateSut();
            var rawResult = await sut.GetJobs(request, TestContext.Current.CancellationToken);

            AssertResultValue<OkObjectResult, PagedList<JobResponseModel>>(rawResult, expected);

            await JobsService.Received(1).GetJobsAsync(Arg.Any<GetJobsRequest>(), Arg.Any<CancellationToken>());
        }
    }

    public class GetJobById : JobControllerTestsBase
    {
        [Fact]
        public async Task ShouldReturnOk_WhenSuccess()
        {
            const string id = "test-id";
            var jobModel = GetJobModel(id: id);

            JobsService.GetJobByIdAsync(Arg.Is<string>(s => s == id), Arg.Any<CancellationToken>())
                .Returns(jobModel);

            var expected = jobModel.ToJobResponseDetailModel();

            var sut = GetOrCreateSut();
            var rawResult = await sut.GetJobById(id, TestContext.Current.CancellationToken);

            AssertResultValue<OkObjectResult, JobResponseDetailModel>(rawResult, expected);
            AssertEtagSet(jobModel.ConcurrencyToken);

            await JobsService.Received(1).GetJobByIdAsync(Arg.Is<string>(s => s == id), Arg.Any<CancellationToken>());
        }
    }

    public class CreateJob : JobControllerTestsBase
    {
        [Fact]
        public async Task ShouldReturnAccepted_WhenSuccess()
        {
            const string idempotencyKey = "test-idempotency-key";

            var requestModel = new CreateJobRequestModel(nameof(JobTypeEnum.ArchiveUserGroupJob), null);

            var jobModel = GetJobModel(idempotencyKey: idempotencyKey);

            JobsService.CreateJobAsync(Arg.Any<CreateJobRequest>(), Arg.Any<CancellationToken>())
                .Returns(jobModel);

            var expected = jobModel.ToJobResponseModel();

            var sut = GetOrCreateSut();
            var rawResult = await sut.CreateJob(requestModel, idempotencyKey, TestContext.Current.CancellationToken);

            AssertResultValue<AcceptedAtActionResult, JobResponseModel>(rawResult, expected);
            AssertEtagSet(jobModel.ConcurrencyToken);

            await JobsService.Received(1).CreateJobAsync(Arg.Is<CreateJobRequest>(r => r.IdempotencyKey == idempotencyKey), Arg.Any<CancellationToken>());
        }
    }

    public class UpdateJob : JobControllerTestsBase
    {
        [Fact]
        public async Task ShouldReturnOk_WhenSuccess()
        {
            const string id = "test-id";
            const string concurrencyToken = "test-concurrency-token";
            var requestModel = new UpdateJobRequestModel(nameof(JobStatusEnum.Failed), "1010", "BAD THING HAPPEN");

            var jobModel = GetJobModel(id: id, concurrencyToken: concurrencyToken, status: requestModel.Status, errorCode: requestModel.ErrorCode, errorMessage: requestModel.ErrorMessage);

            JobsService.UpdateJobAsync(Arg.Any<UpdateJobRequest>(), Arg.Any<CancellationToken>())
                .Returns(jobModel);

            var expected = jobModel.ToJobResponseModel();

            var sut = GetOrCreateSut();
            var rawResult = await sut.UpdateJob(id, concurrencyToken, requestModel, TestContext.Current.CancellationToken);

            AssertResultValue<OkObjectResult, JobResponseModel>(rawResult, expected);
            AssertEtagSet(jobModel.ConcurrencyToken);

            await JobsService.Received(1)
                .UpdateJobAsync(Arg.Is<UpdateJobRequest>(r => r.Id == id && r.ConcurrencyToken == concurrencyToken),
                    Arg.Any<CancellationToken>());
        }
    }
}