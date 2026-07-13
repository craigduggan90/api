using Sol.Common.Extensions;
using Sol.Common.Providers.Identifiers;
using Sol.Common.Providers.Temporal;
using Sol.Domain.Entities;
using Sol.Domain.Enums;
using System.Text.Json;

namespace Sol.Domain.UnitTests.Entities;

public static class JobTests
{
    public abstract class JobTestsBase
    {
        protected const JobTypeEnum DefaultType = JobTypeEnum.ArchiveProjectJob;
        protected const string DefaultParameters = """{"foo":"bar"}""";
        protected readonly DateTimeOffset BaseDate = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        protected static Job CreateJob(Action<Job>? setup = null)
        {
            var job = new Job(DefaultType, DefaultParameters);
            setup?.Invoke(job);
            return job;
        }
    }

    public class ConstructorTests : JobTestsBase
    {
        [Fact]
        public void SetsTypeAndParameters_WhenConstructed()
        {
            var job = new Job(DefaultType, DefaultParameters);

            Assert.Equal(DefaultType, job.Type);
            Assert.Equal(DefaultParameters, job.Parameters);
        }

        [Fact]
        public void SetsStatusToPending_WhenConstructed()
        {
            var job = CreateJob();

            Assert.Equal(JobStatusEnum.Pending, job.Status);
        }

        [Fact]
        public void SetsErrorFieldsToNull_WhenConstructed()
        {
            var job = CreateJob();

            Assert.Null(job.ErrorCode);
            Assert.Null(job.ErrorMessage);
        }

        [Fact]
        public void SetsLastEventTimeToNull_WhenConstructed()
        {
            var job = CreateJob();

            Assert.Null(job.LastEventTime);
        }

        [Fact]
        public void SetsId_WhenIdentifierProviderIsFixed()
        {
            using var _ = new IdentifierProviderContext("fixed-id");

            var job = CreateJob();

            Assert.Equal("fixed-id", job.Id);
        }

        [Fact]
        public void SetsDateCreatedAndDateModified_WhenTimeProviderIsFixed()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);

            var job = CreateJob();

            Assert.Equal(BaseDate.UtcDateTime, job.DateCreated);
            Assert.Equal(BaseDate.UtcDateTime, job.DateModified);
        }

        [Fact]
        public void SetsParametersToNull_WhenParametersArgumentIsNull()
        {
            var job = new Job(DefaultType, null);

            Assert.Null(job.Parameters);
        }
    }

    public class UpdateTests : JobTestsBase
    {
        [Fact]
        public void AppliesUpdate_WhenLastEventTimeIsNull()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);
            var job = CreateJob();
            var eventTime = DateTimeOffsetProvider.Now.UtcDateTime;

            job.Update(eventTime, JobStatusEnum.InProgress, null, null);

            Assert.Equal(JobStatusEnum.InProgress, job.Status);
            Assert.Equal(eventTime, job.LastEventTime);
        }

        [Fact]
        public void AppliesUpdate_WhenEventTimeIsNewerThanLastEventTime()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);
            var firstEventTime = DateTimeOffsetProvider.Now.UtcDateTime;
            var job = CreateJob(j => j.Update(firstEventTime, JobStatusEnum.InProgress, null, null));
            var secondEventTime = firstEventTime.AddMinutes(1);

            job.Update(secondEventTime, JobStatusEnum.Complete, null, null);

            Assert.Equal(JobStatusEnum.Complete, job.Status);
            Assert.Equal(secondEventTime, job.LastEventTime);
        }

        [Fact]
        public void DoesNotApplyUpdate_WhenEventTimeIsOlderThanLastEventTime()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);
            var firstEventTime = DateTimeOffsetProvider.Now.UtcDateTime;
            var job = CreateJob(j => j.Update(firstEventTime, JobStatusEnum.Complete, null, null));
            var staleEventTime = firstEventTime.AddMinutes(-1);

            job.Update(staleEventTime, JobStatusEnum.Failed, "SOME_CODE", "some message");

            Assert.Equal(JobStatusEnum.Complete, job.Status);
            Assert.Null(job.ErrorCode);
            Assert.Null(job.ErrorMessage);
            Assert.Equal(firstEventTime, job.LastEventTime);
        }

        [Fact]
        public void DoesNotApplyUpdate_WhenEventTimeEqualsLastEventTime()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);
            var eventTime = DateTimeOffsetProvider.Now.UtcDateTime;
            var job = CreateJob(j => j.Update(eventTime, JobStatusEnum.InProgress, null, null));

            job.Update(eventTime, JobStatusEnum.Complete, null, null);

            Assert.Equal(JobStatusEnum.InProgress, job.Status);
        }

        [Fact]
        public void SetsErrorCodeAndMessage_WhenStatusIsFailed()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);
            var job = CreateJob();
            var eventTime = DateTimeOffsetProvider.Now.UtcDateTime;

            job.Update(eventTime, JobStatusEnum.Failed, "SOME_CODE", "some message");

            Assert.Equal(JobStatusEnum.Failed, job.Status);
            Assert.Equal("SOME_CODE", job.ErrorCode);
            Assert.Equal("some message", job.ErrorMessage);
        }
        
        [Fact]
        public void DoesNotChangeErrorFields_WhenStatusIsRepeatedWithNoNewErrorInfo()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);
            var firstEventTime = DateTimeOffsetProvider.Now.UtcDateTime;
            var job = CreateJob(j => j.Update(firstEventTime, JobStatusEnum.Failed, "SOME_CODE", "some message"));
            var secondEventTime = firstEventTime.AddMinutes(1);

            job.Update(secondEventTime, JobStatusEnum.Failed, "SOME_CODE", "some message");

            Assert.Equal(JobStatusEnum.Failed, job.Status);
            Assert.Equal("SOME_CODE", job.ErrorCode);
            Assert.Equal("some message", job.ErrorMessage);
            Assert.Equal(secondEventTime, job.LastEventTime);
        }

        [Fact]
        public void AdvancesLastEventTime_WhenNoOtherFieldChanges()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);
            var firstEventTime = DateTimeOffsetProvider.Now.UtcDateTime;
            var job = CreateJob(j => j.Update(firstEventTime, JobStatusEnum.InProgress, null, null));
            var secondEventTime = firstEventTime.AddMinutes(1);

            job.Update(secondEventTime, JobStatusEnum.InProgress, null, null);

            Assert.Equal(secondEventTime, job.LastEventTime);
        }
    }

    public class DeleteTests : JobTestsBase
    {
        [Fact]
        public void SetsDateDeleted_WhenCalled()
        {
            var fixedTime = BaseDate;
            using var _ = new DateTimeOffsetProviderContext(fixedTime);
            var job = CreateJob();

            job.Delete();

            Assert.Equal(fixedTime.UtcDateTime, job.DateDeleted);
        }

        [Fact]
        public void DoesNotChangeDateDeleted_WhenCalledTwice()
        {
            using var firstDeleteTime = new DateTimeOffsetProviderContext(BaseDate);
            var job = CreateJob(j => j.Delete());
            var firstDeletedAt = job.DateDeleted;

            using var secondDeleteTime = new DateTimeOffsetProviderContext(BaseDate.AddSeconds(1));
            job.Delete();

            Assert.Equal(firstDeletedAt, job.DateDeleted);
        }
    }

    public class AsSerializableTests : JobTestsBase
    {
        [Fact]
        public void ReflectsCurrentErrorCode_AfterJobFails()
        {
            var eventTime = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var job = CreateJob(j => j.Update(eventTime, JobStatusEnum.Failed, "SOME_CODE", "some message"));

            var serializable = job.AsSerializable();
            var errorCode = serializable.GetType().GetProperty("ErrorCode")!.GetValue(serializable);

            Assert.Equal(job.ErrorCode, errorCode);
            Assert.NotEqual(job.ErrorMessage, errorCode); // guards against an ErrorCode/ErrorMessage mix-up
        }

        [Fact]
        public void ReflectsCurrentStatus_AfterUpdate()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);
            var job = CreateJob();
            job.Update(DateTimeOffsetProvider.Now.UtcDateTime, JobStatusEnum.InProgress, null, null);

            var serializable = job.AsSerializable();
            var status = serializable.GetType().GetProperty("Status")!.GetValue(serializable);

            Assert.Equal(job.Status, status);
        }

        [Fact]
        public void ReflectsCurrentLastEventTime_AfterUpdate()
        {
            using var _ = new DateTimeOffsetProviderContext(BaseDate);
            var eventTime = DateTimeOffsetProvider.Now.UtcDateTime;
            var job = CreateJob(j => j.Update(eventTime, JobStatusEnum.InProgress, null, null));

            var serializable = job.AsSerializable();
            var lastEventTime = serializable.GetType().GetProperty("LastEventTime")!.GetValue(serializable);

            Assert.Equal(job.LastEventTime, lastEventTime);
        }
    }
}