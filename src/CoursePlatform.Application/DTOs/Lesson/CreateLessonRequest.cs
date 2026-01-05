namespace CoursePlatform.Application.DTOs.Lesson;

public class CreateLessonRequest
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
}