using CoursePlatform.Application.Common;
using CoursePlatform.Application.DTOs.Lesson;

namespace CoursePlatform.Application.Interfaces;

public interface ILessonService
{
    Task<Result<LessonResponse>> CreateAsync(CreateLessonRequest request, CancellationToken cancellationToken = default);
    Task<Result<LessonResponse>> UpdateAsync(Guid id, UpdateLessonRequest request, CancellationToken cancellationToken = default);
    Task<Result<LessonResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LessonResponse>>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> ReorderAsync(Guid courseId, List<ReorderLessonRequest> requests, CancellationToken cancellationToken = default);
    Task<Result> MoveUpAsync(Guid lessonId, CancellationToken cancellationToken = default);
    Task<Result> MoveDownAsync(Guid lessonId, CancellationToken cancellationToken = default);
}