namespace CoursePlatform.Application.DTOs.Lesson;

public class ReorderLessonRequest
{
    public Guid LessonId { get; set; }
    public int NewOrder { get; set; }
}