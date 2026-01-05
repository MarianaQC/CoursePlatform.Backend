namespace CoursePlatform.Application.DTOs.Course;

public class CourseSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}