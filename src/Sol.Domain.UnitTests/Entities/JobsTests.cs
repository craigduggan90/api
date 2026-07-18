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
        protected const string DefaultIdempotencyKey = "idempotency-key-001";
        protected const JobTypeEnum DefaultType = JobTypeEnum.ArchiveProjectJob;
        protected const string DefaultParameters = """{"foo":"bar"}""";

        protected static Job CreateJob(Action<Job>? setup = null)
        {
            var job = new Job(DefaultIdempotencyKey, DefaultType, DefaultParameters);
            setup?.Invoke(job);
            return job;
        }
    }

    public class Constructor : JobTestsBase
    {
        [Fact]
        public void SetsIdempotencyKeyTypeAndParameters_WhenConstructed()
        {
            var job = new Job(DefaultIdempotencyKey, DefaultType, DefaultParameters);

            Assert.Equal(DefaultIdempotencyKey, job.IdempotencyKey);
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
        public void SetsParametersToNull_WhenParametersArgumentIsNull()
        {
            var job = new Job(DefaultIdempotencyKey, DefaultType, null);

            Assert.Null(job.Parameters);
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
            var fixedTime = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
            using var _ = new DateTimeOffsetProviderContext(fixedTime);

            var job = CreateJob();

            Assert.Equal(fixedTime.UtcDateTime, job.DateCreated);
            Assert.Equal(fixedTime.UtcDateTime, job.DateModified);
        }
    }

    public class Update : JobTestsBase
    {
        [Fact]
        public void SetsStatus_WhenCalled()
        {
            var job = CreateJob();

            job.Update(JobStatusEnum.InProgress, null, null);

            Assert.Equal(JobStatusEnum.InProgress, job.Status);
        }

        [Fact]
        public void SetsErrorCodeAndMessage_WhenStatusIsFailed()
        {
            var job = CreateJob();

            job.Update(JobStatusEnum.Failed, "SOME_CODE", "some message");

            Assert.Equal(JobStatusEnum.Failed, job.Status);
            Assert.Equal("SOME_CODE", job.ErrorCode);
            Assert.Equal("some message", job.ErrorMessage);
        }

        [Fact]
        public void MarksJobAsDirty_WhenValueChanges()
        {
            var job = CreateJob();

            job.Update(JobStatusEnum.InProgress, null, null);

            Assert.True(job.IsDirty);
        }

        [Fact]
        public void DoesNotMarkJobAsDirty_WhenValuesAreUnchanged()
        {
            var job = CreateJob();

            job.Update(JobStatusEnum.Pending, null, null);

            Assert.False(job.IsDirty);
        }

        [Fact]
        public void AllowsStatusToBeAppliedRepeatedly_WhenCalledMultipleTimes()
        {
            var job = CreateJob();

            job.Update(JobStatusEnum.InProgress, null, null);
            job.Update(JobStatusEnum.Complete, null, null);

            Assert.Equal(JobStatusEnum.Complete, job.Status);
        }
        
        [Fact]
        public void DoesNotChangeStatus_WhenSameStatusIsProvidedAgain()
        {
            var job = CreateJob(j => j.Update(JobStatusEnum.Failed, "SOME_CODE", "some message"));

            job.Update(JobStatusEnum.Failed, "OTHER_CODE", "some other message");

            Assert.Equal(JobStatusEnum.Failed, job.Status);
            Assert.Equal("OTHER_CODE", job.ErrorCode);
            Assert.Equal("some other message", job.ErrorMessage);
        }

        [Fact]
        public void MarksJobAsDirty_WhenOnlyErrorFieldsChange_EvenIfStatusIsUnchanged()
        {
            var job = CreateJob(j => j.Update(JobStatusEnum.Failed, "SOME_CODE", "some message"));

            job.Update(JobStatusEnum.Failed, "OTHER_CODE", "some other message");

            Assert.True(job.IsDirty);
        }
    }
    
    public class ConcurrencyTokenTests : JobTestsBase
    {
        [Fact]
        public void IsSet_WhenJustConstructed()
        {
            var job = CreateJob();

            Assert.False(string.IsNullOrEmpty(job.ConcurrencyToken));
        }

        [Fact]
        public void Changes_WhenUpdateChangesAValue()
        {
            var job = CreateJob();
            var initialToken = job.ConcurrencyToken;

            job.Update(JobStatusEnum.InProgress, null, null);

            Assert.NotEqual(initialToken, job.ConcurrencyToken);
        }

        [Fact]
        public void DoesNotChange_WhenUpdateIsANoOp()
        {
            var job = CreateJob();
            job.Update(JobStatusEnum.InProgress, null, null);
            var tokenAfterFirstUpdate = job.ConcurrencyToken;

            job.Update(JobStatusEnum.InProgress, null, null);

            Assert.Equal(tokenAfterFirstUpdate, job.ConcurrencyToken);
        }

        [Fact]
        public void Changes_WhenJobIsDeleted()
        {
            var job = CreateJob();
            var tokenBeforeDelete = job.ConcurrencyToken;

            job.Delete();

            Assert.NotEqual(tokenBeforeDelete, job.ConcurrencyToken);
        }
    }

    public class Delete : JobTestsBase
    {
        [Fact]
        public void SetsDateDeleted_WhenCalled()
        {
            var fixedTime = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
            using var _ = new DateTimeOffsetProviderContext(fixedTime);
            var job = CreateJob();

            job.Delete();

            Assert.Equal(fixedTime.UtcDateTime, job.DateDeleted);
        }

        [Fact]
        public void MarksJobAsDirty_WhenCalled()
        {
            var job = CreateJob();

            job.Delete();

            Assert.True(job.IsDirty);
        }

        [Fact]
        public void DoesNotChangeDateDeleted_WhenCalledTwice()
        {
            using var firstDeleteTime = new DateTimeOffsetProviderContext(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero));
            var job = CreateJob(j => j.Delete());
            var firstDeletedAt = job.DateDeleted;

            using var secondDeleteTime = new DateTimeOffsetProviderContext(new DateTimeOffset(2026, 1, 1, 12, 0, 1, TimeSpan.Zero));
            job.Delete();

            Assert.Equal(firstDeletedAt, job.DateDeleted);
        }
    }

    public class ToStringTests : JobTestsBase
    {
        [Fact]
        public void ReturnsValidJson_WhenCalled()
        {
            var job = CreateJob();

            var json = job.ToString();

            var exception = Record.Exception(() => JsonDocument.Parse(json));
            Assert.Null(exception);
        }

        [Fact]
        public void DoesNotIncludeParametersOrErrorMessage_WhenCalled()
        {
            var job = CreateJob();

            var json = job.ToString();
            var root = JsonDocument.Parse(json).RootElement;

            Assert.False(root.TryGetProperty("parameters", out _));
            Assert.False(root.TryGetProperty("errorMessage", out _));
        }

        [Fact]
        public void IncludesIdIdempotencyKeyAndTimestamps_WhenCalled()
        {
            var job = CreateJob();

            var json = job.ToString();
            var root = JsonDocument.Parse(json).RootElement;

            Assert.Equal(job.Id, root.GetProperty("id").GetString());
            Assert.Equal(job.IdempotencyKey, root.GetProperty("idempotencyKey").GetString());
            Assert.True(root.TryGetProperty("dateCreated", out _));
            Assert.True(root.TryGetProperty("dateModified", out _));
        }

        [Fact]
        public void IncludesErrorCodeAsNull_WhenJobHasNotFailed()
        {
            var job = CreateJob();

            var json = job.ToString();
            var root = JsonDocument.Parse(json).RootElement;

            Assert.Equal(JsonValueKind.Null, root.GetProperty("errorCode").ValueKind);
        }

        [Fact]
        public void IncludesErrorCode_WhenJobHasFailed()
        {
            var job = CreateJob(j => j.Update(JobStatusEnum.Failed, "SOME_CODE", "some message"));

            var json = job.ToString();
            var root = JsonDocument.Parse(json).RootElement;

            Assert.Equal("SOME_CODE", root.GetProperty("errorCode").GetString());
        }

        [Fact]
        public void SerializesTypeAsString_NotNumber()
        {
            var job = CreateJob();

            var json = job.ToString();
            var root = JsonDocument.Parse(json).RootElement;

            Assert.Equal(JsonValueKind.String, root.GetProperty("type").ValueKind);
            Assert.Equal("archiveProjectJob", root.GetProperty("type").GetString());
        }

        [Fact]
        public void SerializesStatusAsString_NotNumber()
        {
            var job = CreateJob();

            var json = job.ToString();
            var root = JsonDocument.Parse(json).RootElement;

            Assert.Equal(JsonValueKind.String, root.GetProperty("status").ValueKind);
            Assert.Equal("pending", root.GetProperty("status").GetString());
        }
    }
}