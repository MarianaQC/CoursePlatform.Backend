using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Interfaces;
using CoursePlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Repositories;

public class LessonRepository : Repository<Lesson>, ILessonRepository
{
    public LessonRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsOrderUniqueAsync(
        Guid courseId, 
        int order, 
        Guid? excludeLessonId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(l => l.CourseId == courseId && l.Order == order);

        if (excludeLessonId.HasValue)
        {
            query = query.Where(l => l.Id != excludeLessonId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<int> GetMaxOrderByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var maxOrder = await _dbSet
            .Where(l => l.CourseId == courseId)
            .MaxAsync(l => (int?)l.Order, cancellationToken);

        return maxOrder ?? 0;
    }

    public async Task<Lesson> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
}