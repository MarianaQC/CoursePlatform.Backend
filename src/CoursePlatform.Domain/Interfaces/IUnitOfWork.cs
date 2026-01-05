namespace CoursePlatform.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICourseRepository Courses { get; }
    ILessonRepository Lessons { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}