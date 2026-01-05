using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
    
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    
    public bool CanPublish()
    {
        return Lessons.Any(l => !l.IsDeleted);
    }
    
    public void Publish()
    {
        if (CanPublish())
        {
            Status = CourseStatus.Published;
        }
    }
    
    public void Unpublish()
    {
        Status = CourseStatus.Draft;
    }
    
    public void SoftDelete()
    {
        IsDeleted = true;
    }
}