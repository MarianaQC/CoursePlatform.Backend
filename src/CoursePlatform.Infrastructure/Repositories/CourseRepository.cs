using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Domain.Interfaces;
using CoursePlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Repositories;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Course> GetByIdWithLessonsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<Course> Courses, int TotalCount)> SearchAsync(
        string query, 
        CourseStatus? status, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbSet.Include(c => c.Lessons).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            queryable = queryable.Where(c => c.Title.Contains(query));
        }

        if (status.HasValue)
        {
            queryable = queryable.Where(c => c.Status == status.Value);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        var courses = await queryable
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (courses, totalCount);
    }

    public async Task<Course> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}