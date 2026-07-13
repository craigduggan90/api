using Sol.Common.Providers.Identifiers;
using Sol.Common.Providers.Temporal;
using Sol.Data.Repositories.Jobs;
using Sol.Domain.Entities;
using Sol.Domain.Enums;

namespace Sol.Data.UnitTests.Repositories.Jobs;

public static class JobsFilterHelperTests
{
    private static readonly DateTime BaseDate = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private static readonly JobStatusEnum[] Statuses = [JobStatusEnum.Pending, JobStatusEnum.InProgress, JobStatusEnum.Complete, JobStatusEnum.Failed];
    private static readonly JobTypeEnum[] Types = [JobTypeEnum.ArchiveProjectJob, JobTypeEnum.ArchiveUserGroupJob];

    private static IQueryable<Job> GetSeedData(int count) => Enumerable.Range(1, count)
        .Select(i =>
        {
            using var idFix = new IdentifierProviderContext($"{i:D3}");
            using var dtFix = new DateTimeOffsetProviderContext(BaseDate.AddDays(i - 1));

            var job = new Job($"idempotency-key-{i:D3}", Types[i % Types.Length], null);
            var status = Statuses[i % Statuses.Length];
            var errorCode = status == JobStatusEnum.Failed ? $"ERR-{i:D3}" : null;
            var errorMessage = status == JobStatusEnum.Failed ? $"error-message-{i:D3}" : null;

            job.Update(BaseDate.AddDays(i - 1), status, errorCode, errorMessage);
            return job;
        })
        .AsQueryable();

    public class ApplyTypeFilter
    {
        [Fact]
        public void ShouldNotApplyFilter_WhenNoValueProvided()
        {
            var data = GetSeedData(30);
            var filtered = data.ApplyTypeFilter(null);
            Assert.Same(data, filtered);
        }

        [Fact]
        public void ShouldApplyFilter_WhenValueProvided()
        {
            const JobTypeEnum value = JobTypeEnum.ArchiveProjectJob;
            var data = GetSeedData(30);
            var expected = data.Where(job => job.Type == value);
            var filtered = data.ApplyTypeFilter(value);
            Assert.Equivalent(expected, filtered, true);
        }
    }

    public class ApplyStatusFilter
    {
        [Fact]
        public void ShouldNotApplyFilter_WhenNoValueProvided()
        {
            var data = GetSeedData(30);
            var filtered = data.ApplyStatusFilter(null);
            Assert.Same(data, filtered);
        }

        [Fact]
        public void ShouldApplyFilter_WhenValueProvided()
        {
            const JobStatusEnum value = JobStatusEnum.Failed;
            var data = GetSeedData(30);
            var expected = data.Where(job => job.Status == value);
            var filtered = data.ApplyStatusFilter(value);
            Assert.Equivalent(expected, filtered, true);
        }
    }

    public class ApplyErrorCodeFilter
    {
        [Fact]
        public void ShouldNotApplyFilter_WhenNoValueProvided()
        {
            var data = GetSeedData(30);
            var filtered = data.ApplyErrorCodeFilter(null);
            Assert.Same(data, filtered);
        }

        [Fact]
        public void ShouldApplyFilter_WhenValueProvided()
        {
            const string value = "ERR-004";
            var data = GetSeedData(30);
            var expected = data.Where(job => job.ErrorCode == value);
            var filtered = data.ApplyErrorCodeFilter(value);
            Assert.Equivalent(expected, filtered, true);
        }
    }

    public class ApplyLastEventTimeFromFilter
    {
        [Fact]
        public void ShouldNotApplyFilter_WhenNoValueProvided()
        {
            var data = GetSeedData(30);
            var filtered = data.ApplyLastEventTimeFromFilter(null);
            Assert.Same(data, filtered);
        }

        [Fact]
        public void ShouldApplyFilter_WhenValueProvided()
        {
            var value = BaseDate.AddDays(14);
            var data = GetSeedData(30);
            var expected = data.Where(job => job.LastEventTime >= value);
            var filtered = data.ApplyLastEventTimeFromFilter(value);
            Assert.Equivalent(expected, filtered, true);
        }

        [Fact]
        public void ShouldIncludeExactMatch_WhenValueEqualsLastEventTime()
        {
            var value = BaseDate.AddDays(14);
            var data = GetSeedData(30);
            var filtered = data.ApplyLastEventTimeFromFilter(value);
            Assert.Contains(filtered, job => job.LastEventTime == value);
        }
    }

    public class ApplyLastEventTimeToFilter
    {
        [Fact]
        public void ShouldNotApplyFilter_WhenNoValueProvided()
        {
            var data = GetSeedData(30);
            var filtered = data.ApplyLastEventTimeToFilter(null);
            Assert.Same(data, filtered);
        }

        [Fact]
        public void ShouldApplyFilter_WhenValueProvided()
        {
            var value = BaseDate.AddDays(14);
            var data = GetSeedData(30);
            var expected = data.Where(job => job.LastEventTime < value);
            var filtered = data.ApplyLastEventTimeToFilter(value);
            Assert.Equivalent(expected, filtered, true);
        }

        [Fact]
        public void ShouldExcludeExactMatch_WhenValueEqualsLastEventTime()
        {
            var value = BaseDate.AddDays(14);
            var data = GetSeedData(30);
            var filtered = data.ApplyLastEventTimeToFilter(value);
            Assert.DoesNotContain(filtered, job => job.LastEventTime == value);
        }
    }
}