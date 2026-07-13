using Microsoft.EntityFrameworkCore;
using Sol.Data.Repositories.Jobs;
using Sol.Domain.Enums;

namespace Sol.Data.UnitTests.Repositories.Jobs;

public static class JobsRepositoryTests
{
    public class CreateAsync : JobsRepositoryTestsBase
    {
        private JobsRepository CreateSut() => new(Context);

        [Fact]
        public async Task Should_AddEntity_ToChangeTracker()
        {
            var entity = new Domain.Entities.Job("idempotency-key-new", JobTypeEnum.ArchiveProjectJob, null);
            var sut = CreateSut();
            _ = await sut.CreateAsync(entity, TestContext.Current.CancellationToken);

            var tracked = Context.ChangeTracker.Entries<Domain.Entities.Job>()
                .Single(entry => entry.State == EntityState.Added)
                .Entity;

            Assert.Same(entity, tracked);
        }
    }

    public class UpdateAsync : JobsRepositoryTestsBase
    {
        private JobsRepository CreateSut() => new(Context);

        [Fact]
        public async Task Should_UpdateEntity_InChangeTracker()
        {
            var entity = Context.Jobs.Skip(15).First();
            entity.Update(BaseDate.AddDays(31), JobStatusEnum.InProgress, null, null);

            var sut = CreateSut();
            _ = await sut.UpdateAsync(entity, TestContext.Current.CancellationToken);

            var tracked = Context.ChangeTracker.Entries<Domain.Entities.Job>()
                .Single(entry => entry.State == EntityState.Modified)
                .Entity;

            Assert.Same(entity, tracked);
        }
    }

    public class DeleteAsync : JobsRepositoryTestsBase
    {
        private JobsRepository CreateSut() => new(Context);

        [Fact]
        public async Task Should_DeleteEntity_InChangeTracker()
        {
            var entity = Context.Jobs.Skip(15).First();

            var sut = CreateSut();
            _ = await sut.DeleteAsync(entity, TestContext.Current.CancellationToken);

            var tracked = Context.ChangeTracker.Entries<Domain.Entities.Job>()
                .Single(entry => entry.State == EntityState.Deleted)
                .Entity;

            Assert.Same(entity, tracked);
        }
    }
}