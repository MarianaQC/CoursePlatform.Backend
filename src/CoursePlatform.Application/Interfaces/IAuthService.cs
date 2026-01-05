using CoursePlatform.Application.Common;
using CoursePlatform.Application.DTOs.Auth;

namespace CoursePlatform.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}