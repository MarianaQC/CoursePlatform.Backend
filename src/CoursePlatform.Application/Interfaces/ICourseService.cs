using CoursePlatform.Application.Common;
using CoursePlatform.Application.DTOs.Course;

namespace CoursePlatform.Application.Interfaces;

public interface ICourseService
{
    Task<Result<CourseResponse>> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);
    Task<Result<CourseResponse>> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken = default);
    Task<Result<CourseResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<CourseResponse>>> SearchAsync(CourseSearchRequest request, CancellationToken cancellationToken = default);
    Task<Result<CourseSummaryResponse>> GetSummaryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> UnpublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}