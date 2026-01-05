using CoursePlatform.Application.DTOs.Lesson;

namespace CoursePlatform.Application.DTOs.Course;

public class CourseSummaryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public DateTime LastModified { get; set; }
    public List<LessonResponse> Lessons { get; set; } = new();
}