using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Domain.Interfaces;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> IsOrderUniqueAsync(Guid courseId, int order, Guid? excludeLessonId = null, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<Lesson> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default);
}