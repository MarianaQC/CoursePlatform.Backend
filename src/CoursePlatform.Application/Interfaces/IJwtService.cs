using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Interfaces;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
    DateTime GetExpirationDate();
}