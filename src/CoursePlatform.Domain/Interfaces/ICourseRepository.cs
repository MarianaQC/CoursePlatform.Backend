using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course> GetByIdWithLessonsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Course> Courses, int TotalCount)> SearchAsync(
        string query, 
        CourseStatus? status, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    Task<Course> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default);
}