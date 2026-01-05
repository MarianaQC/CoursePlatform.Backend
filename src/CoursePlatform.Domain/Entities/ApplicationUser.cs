using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
}