using CoursePlatform.Application.Common;
using CoursePlatform.Application.Common.Errors;
using CoursePlatform.Application.DTOs.Lesson;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Application.Mappings;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Interfaces;

namespace CoursePlatform.Application.Services;

public class LessonService : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;

    public LessonService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LessonResponse>> CreateAsync(CreateLessonRequest request, CancellationToken cancellationToken = default)
    {
        var courseExists = await _unitOfWork.Courses.ExistsAsync(request.CourseId, cancellationToken);
        if (!courseExists)
        {
            return Result.Failure<LessonResponse>(DomainErrors.Lesson.CourseNotFound);
        }

        var isOrderUnique = await _unitOfWork.Lessons.IsOrderUniqueAsync(request.CourseId, request.Order, null, cancellationToken);
        if (!isOrderUnique)
        {
            return Result.Failure<LessonResponse>(DomainErrors.Lesson.DuplicateOrder);
        }

        var lesson = new Lesson
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            Title = request.Title,
            Order = request.Order,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Lessons.AddAsync(lesson, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(lesson.ToLessonResponse());
    }

    public async Task<Result<LessonResponse>> UpdateAsync(Guid id, UpdateLessonRequest request, CancellationToken cancellationToken = default)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(id, cancellationToken);
        if (lesson == null)
        {
            return Result.Failure<LessonResponse>(DomainErrors.Lesson.NotFound);
        }

        var isOrderUnique = await _unitOfWork.Lessons.IsOrderUniqueAsync(lesson.CourseId, request.Order, id, cancellationToken);
        if (!isOrderUnique)
        {
            return Result.Failure<LessonResponse>(DomainErrors.Lesson.DuplicateOrder);
        }

        lesson.Title = request.Title;
        lesson.Order = request.Order;
        lesson.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Lessons.Update(lesson);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(lesson.ToLessonResponse());
    }

    public async Task<Result<LessonResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(id, cancellationToken);
        if (lesson == null)
        {
            return Result.Failure<LessonResponse>(DomainErrors.Lesson.NotFound);
        }

        return Result.Success(lesson.ToLessonResponse());
    }

    public async Task<Result<IEnumerable<LessonResponse>>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var lessons = await _unitOfWork.Lessons.GetByCourseIdAsync(courseId, cancellationToken);
        var orderedLessons = lessons.OrderBy(l => l.Order).Select(l => l.ToLessonResponse());

        return Result.Success(orderedLessons);
    }

    public async Task<Result> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(id, cancellationToken);
        if (lesson == null)
        {
            return Result.Failure(DomainErrors.Lesson.NotFound);
        }

        lesson.SoftDelete();
        _unitOfWork.Lessons.Update(lesson);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdIncludingDeletedAsync(id, cancellationToken);
        if (lesson == null)
        {
            return Result.Failure(DomainErrors.Lesson.NotFound);
        }

        _unitOfWork.Lessons.Delete(lesson);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ReorderAsync(Guid courseId, List<ReorderLessonRequest> requests, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var request in requests)
            {
                var lesson = await _unitOfWork.Lessons.GetByIdAsync(request.LessonId, cancellationToken);
                if (lesson == null || lesson.CourseId != courseId)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure(DomainErrors.Lesson.NotFound);
                }

                lesson.Order = request.NewOrder;
                lesson.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Lessons.Update(lesson);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> MoveUpAsync(Guid lessonId, CancellationToken cancellationToken = default)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId, cancellationToken);
        if (lesson == null)
        {
            return Result.Failure(DomainErrors.Lesson.NotFound);
        }

        var lessons = (await _unitOfWork.Lessons.GetByCourseIdAsync(lesson.CourseId, cancellationToken))
            .OrderBy(l => l.Order)
            .ToList();

        var currentIndex = lessons.FindIndex(l => l.Id == lessonId);
        if (currentIndex <= 0)
        {
            return Result.Failure(DomainErrors.Lesson.CannotMoveUp);
        }

        var previousLesson = lessons[currentIndex - 1];
        (lesson.Order, previousLesson.Order) = (previousLesson.Order, lesson.Order);

        _unitOfWork.Lessons.Update(lesson);
        _unitOfWork.Lessons.Update(previousLesson);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> MoveDownAsync(Guid lessonId, CancellationToken cancellationToken = default)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId, cancellationToken);
        if (lesson == null)
        {
            return Result.Failure(DomainErrors.Lesson.NotFound);
        }

        var lessons = (await _unitOfWork.Lessons.GetByCourseIdAsync(lesson.CourseId, cancellationToken))
            .OrderBy(l => l.Order)
            .ToList();

        var currentIndex = lessons.FindIndex(l => l.Id == lessonId);
        if (currentIndex >= lessons.Count - 1)
        {
            return Result.Failure(DomainErrors.Lesson.CannotMoveDown);
        }

        var nextLesson = lessons[currentIndex + 1];
        (lesson.Order, nextLesson.Order) = (nextLesson.Order, lesson.Order);

        _unitOfWork.Lessons.Update(lesson);
        _unitOfWork.Lessons.Update(nextLesson);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}