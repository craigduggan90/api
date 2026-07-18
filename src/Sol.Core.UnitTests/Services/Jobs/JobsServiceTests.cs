using Sol.Core.Exceptions;
using Sol.Core.Services.Jobs;
using Sol.Core.Services.Jobs.Requests;
using Sol.Core.Services.Validation;
using Sol.Data.Repositories.Jobs;
using Sol.Data.Services;
using Sol.Domain.Entities;
using Sol.Domain.Enums;

namespace Sol.Core.UnitTests.Services.Jobs;

public static class JobsServiceTests
{
    public abstract class JobsServiceTestsBase
    {
        protected readonly IReadOnlyJobsRepository Repository = Substitute.For<IReadOnlyJobsRepository>();
        protected readonly IUnitOfWork UnitOfWork = Substitute.For<IUnitOfWork>();
        protected readonly IJobsRepository JobsRepository = Substitute.For<IJobsRepository>();
        protected readonly IValidationService Validator = Substitute.For<IValidationService>();

        protected JobsServiceTestsBase()
        {
            UnitOfWork.Jobs.Returns(JobsRepository);
            JobsRepository.CreateAsync(Arg.Any<Job>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<Job>()!));
        }

        protected JobsService CreateSut() => new(Repository, UnitOfWork, Validator);

        protected static Job CreateJob(
            string idempotencyKey = "idempotency-key-001",
            JobTypeEnum type = JobTypeEnum.ArchiveProjectJob,
            string? parameters = null)
            => new(idempotencyKey, type, parameters);
    }

    public class GetJobsAsync : JobsServiceTestsBase
    {
        [Fact]
        public async Task ValidatesRequest_BeforeQuerying()
        {
            var sut = CreateSut();
            var request = new GetJobsRequest();

            await sut.GetJobsAsync(request, TestContext.Current.CancellationToken);

            await Validator.Received(1).ValidateQueryAsync(request, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task PropagatesValidationException_WhenValidationFails()
        {
            var sut = CreateSut();
            var request = new GetJobsRequest();
            Validator.ValidateQueryAsync(request, Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new QueryValidationException([])));

            await Assert.ThrowsAsync<QueryValidationException>(() =>
                sut.GetJobsAsync(request, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ParsesTypeAndStatus_BeforeQueryingRepository()
        {
            var sut = CreateSut();
            var request = new GetJobsRequest(Type: "ArchiveProjectJob", Status: "Failed");
            Repository.GetAsync(
                    Arg.Any<JobTypeEnum?>(), 
                    Arg.Any<JobStatusEnum?>(), 
                    Arg.Any<string?>(),
                     Arg.Any<Data.Models.DateFilter>(),
                    Arg.Any<Data.Models.PaginationFilter>(), 
                    Arg.Any<CancellationToken>())
                .Returns([]);

            await sut.GetJobsAsync(request, TestContext.Current.CancellationToken);

            await Repository.Received(1).GetAsync(
                JobTypeEnum.ArchiveProjectJob, 
                JobStatusEnum.Failed, 
                null,
                Arg.Any<Data.Models.DateFilter>(), 
                Arg.Any<Data.Models.PaginationFilter>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ReturnsMappedPagedList_WhenJobsExist()
        {
            var sut = CreateSut();
            var request = new GetJobsRequest();
            var job = CreateJob();
            Repository.GetAsync(
                    Arg.Any<JobTypeEnum?>(), 
                    Arg.Any<JobStatusEnum?>(), 
                    Arg.Any<string?>(), 
                    Arg.Any<Data.Models.DateFilter>(),
                    Arg.Any<Data.Models.PaginationFilter>(), 
                    Arg.Any<CancellationToken>())
                .Returns([job]);

            var result = await sut.GetJobsAsync(request, TestContext.Current.CancellationToken);

            Assert.Single(result.Data);
            Assert.Equal(job.Id, result.Data[0].Id);
        }
    }

    public class GetJobByIdAsync : JobsServiceTestsBase
    {
        [Fact]
        public async Task ReturnsMappedJob_WhenJobExists()
        {
            var sut = CreateSut();
            var job = CreateJob();
            Repository.GetByIdAsync(job.Id, Arg.Any<CancellationToken>()).Returns(job);

            var result = await sut.GetJobByIdAsync(job.Id, TestContext.Current.CancellationToken);

            Assert.Equal(job.Id, result.Id);
        }

        [Fact]
        public async Task ThrowsNotFoundException_WhenJobDoesNotExist()
        {
            var sut = CreateSut();
            Repository.GetByIdAsync("missing-id", Arg.Any<CancellationToken>()).Returns((Job?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                sut.GetJobByIdAsync("missing-id", TestContext.Current.CancellationToken));
        }
    }

    public class CreateJobAsync : JobsServiceTestsBase
    {
        [Fact]
        public async Task PropagatesValidationException_WhenValidationFails()
        {
            var sut = CreateSut();
            var request = new CreateJobRequest("idempotency-key-001", "ArchiveProjectJob", null);
            Validator.ValidateCommandAsync(request, Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new CommandValidationException([])));

            await Assert.ThrowsAsync<CommandValidationException>(() =>
                sut.CreateJobAsync(request, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ReturnsExistingJob_WhenIdempotencyKeyAlreadyExists()
        {
            var sut = CreateSut();
            var request = new CreateJobRequest("idempotency-key-001", "ArchiveProjectJob", null);
            var extant = CreateJob(request.IdempotencyKey);
            JobsRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey, Arg.Any<CancellationToken>())
                .Returns(extant);

            var result = await sut.CreateJobAsync(request, TestContext.Current.CancellationToken);

            Assert.Equal(extant.Id, result.Id);
        }

        [Fact]
        public async Task DoesNotCreateNewJob_WhenIdempotencyKeyAlreadyExists()
        {
            var sut = CreateSut();
            var request = new CreateJobRequest("idempotency-key-001", "ArchiveProjectJob", null);
            var extant = CreateJob(request.IdempotencyKey);
            JobsRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey, Arg.Any<CancellationToken>())
                .Returns(extant);

            await sut.CreateJobAsync(request, TestContext.Current.CancellationToken);

            await JobsRepository.DidNotReceive().CreateAsync(Arg.Any<Job>(), Arg.Any<CancellationToken>());
            await UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreatesAndSavesNewJob_WhenIdempotencyKeyDoesNotExist()
        {
            var sut = CreateSut();
            var request = new CreateJobRequest("idempotency-key-001", "ArchiveProjectJob", null);
            JobsRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey, Arg.Any<CancellationToken>())
                .Returns((Job?)null);
            JobsRepository.CreateAsync(Arg.Any<Job>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<Job>()!));

            var result = await sut.CreateJobAsync(request, TestContext.Current.CancellationToken);

            await JobsRepository.Received(1).CreateAsync(
                Arg.Is<Job>(job => job!.IdempotencyKey == request.IdempotencyKey),
                Arg.Any<CancellationToken>());
            await UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            Assert.Equal(request.IdempotencyKey, result.IdempotencyKey);
        }
    }

    public class UpdateJobAsync : JobsServiceTestsBase
    {
        [Fact]
        public async Task ValidatesRequest_BeforeUpdating()
        {
            var sut = CreateSut();
            var job = CreateJob();
            JobsRepository.GetByIdAsync(job.Id, Arg.Any<CancellationToken>()).Returns(job);
            var request = new UpdateJobRequest(job.Id, DateTime.UtcNow, "InProgress", null, null);

            await sut.UpdateJobAsync(request, TestContext.Current.CancellationToken);

            await Validator.Received(1).ValidateCommandAsync(request, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task PropagatesValidationException_WhenValidationFails()
        {
            var sut = CreateSut();
            var request = new UpdateJobRequest("job-id", DateTime.UtcNow, "InProgress", null, null);
            Validator.ValidateCommandAsync(request, Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new CommandValidationException([])));

            await Assert.ThrowsAsync<CommandValidationException>(() =>
                sut.UpdateJobAsync(request, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ThrowsNotFoundException_WhenJobDoesNotExist()
        {
            var sut = CreateSut();
            var request = new UpdateJobRequest("missing-id", DateTime.UtcNow, "InProgress", null, null);
            JobsRepository.GetByIdAsync("missing-id", Arg.Any<CancellationToken>()).Returns((Job?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                sut.UpdateJobAsync(request, TestContext.Current.CancellationToken));
        }
    }
}