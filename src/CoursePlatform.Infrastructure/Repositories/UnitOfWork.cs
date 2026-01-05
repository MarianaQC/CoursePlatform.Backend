using CoursePlatform.Domain.Interfaces;
using CoursePlatform.Infrastructure.Data;

namespace CoursePlatform.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private ICourseRepository _courses;
    private ILessonRepository _lessons;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ICourseRepository Courses
    {
        get
        {
            _courses ??= new CourseRepository(_context);
            return _courses;
        }
    }

    public ILessonRepository Lessons
    {
        get
        {
            _lessons ??= new LessonRepository(_context);
            return _lessons;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.RollbackTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}