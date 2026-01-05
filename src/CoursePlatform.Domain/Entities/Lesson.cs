namespace CoursePlatform.Domain.Entities;

public class Lesson : BaseEntity
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    
    public Course Course { get; set; }
    
    public void SoftDelete()
    {
        IsDeleted = true;
    }
}