using CoursePlatform.Application.DTOs.Course;
using CoursePlatform.Application.DTOs.Lesson;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Mappings;

public static class MappingExtensions
{
    public static CourseResponse ToCourseResponse(this Course course)
    {
        return new CourseResponse
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status.ToString(),
            LessonCount = course.Lessons?.Count(l => !l.IsDeleted) ?? 0,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }

    public static CourseSummaryResponse ToCourseSummaryResponse(this Course course)
    {
        return new CourseSummaryResponse
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status.ToString(),
            TotalLessons = course.Lessons?.Count(l => !l.IsDeleted) ?? 0,
            LastModified = course.UpdatedAt,
            Lessons = course.Lessons?
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.Order)
                .Select(l => l.ToLessonResponse())
                .ToList() ?? new List<LessonResponse>()
        };
    }

    public static LessonResponse ToLessonResponse(this Lesson lesson)
    {
        return new LessonResponse
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Order = lesson.Order,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };
    }
}