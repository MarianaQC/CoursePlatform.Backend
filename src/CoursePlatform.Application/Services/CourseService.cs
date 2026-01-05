using CoursePlatform.Application.Common;
using CoursePlatform.Application.Common.Errors;
using CoursePlatform.Application.DTOs.Course;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Application.Mappings;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Domain.Interfaces;

namespace CoursePlatform.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CourseResponse>> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Status = CourseStatus.Draft,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Courses.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(course.ToCourseResponse());
    }

    public async Task<Result<CourseResponse>> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id, cancellationToken);
        if (course == null)
        {
            return Result.Failure<CourseResponse>(DomainErrors.Course.NotFound);
        }

        course.Title = request.Title;
        course.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(course.ToCourseResponse());
    }

    public async Task<Result<CourseResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetByIdWithLessonsAsync(id, cancellationToken);
        if (course == null)
        {
            return Result.Failure<CourseResponse>(DomainErrors.Course.NotFound);
        }

        return Result.Success(course.ToCourseResponse());
    }

    public async Task<Result<PagedResult<CourseResponse>>> SearchAsync(CourseSearchRequest request, CancellationToken cancellationToken = default)
    {
        CourseStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<CourseStatus>(request.Status, true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        var (courses, totalCount) = await _unitOfWork.Courses.SearchAsync(
            request.Query,
            status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var pagedResult = new PagedResult<CourseResponse>
        {
            Items = courses.Select(c => c.ToCourseResponse()).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return Result.Success(pagedResult);
    }

    public async Task<Result<CourseSummaryResponse>> GetSummaryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetByIdWithLessonsAsync(id, cancellationToken);
        if (course == null)
        {
            return Result.Failure<CourseSummaryResponse>(DomainErrors.Course.NotFound);
        }

        return Result.Success(course.ToCourseSummaryResponse());
    }

    public async Task<Result> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetByIdWithLessonsAsync(id, cancellationToken);
        if (course == null)
        {
            return Result.Failure(DomainErrors.Course.NotFound);
        }

        if (!course.CanPublish())
        {
            return Result.Failure(DomainErrors.Course.CannotPublish);
        }

        course.Publish();
        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UnpublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id, cancellationToken);
        if (course == null)
        {
            return Result.Failure(DomainErrors.Course.NotFound);
        }

        course.Unpublish();
        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id, cancellationToken);
        if (course == null)
        {
            return Result.Failure(DomainErrors.Course.NotFound);
        }

        course.SoftDelete();
        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetByIdIncludingDeletedAsync(id, cancellationToken);
        if (course == null)
        {
            return Result.Failure(DomainErrors.Course.NotFound);
        }

        _unitOfWork.Courses.Delete(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}