using CoursePlatform.Application.Common;
using CoursePlatform.Application.Common.Errors;
using CoursePlatform.Application.DTOs.Auth;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result.Failure<AuthResponse>(DomainErrors.Auth.UserAlreadyExists);
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AuthResponse>($"{DomainErrors.Auth.RegistrationFailed}: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "User");

        var token = await _jwtService.GenerateTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var response = new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Expiration = _jwtService.GetExpirationDate(),
            Roles = roles.ToList()
        };

        return Result.Success(response);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure<AuthResponse>(DomainErrors.Auth.InvalidCredentials);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return Result.Failure<AuthResponse>(DomainErrors.Auth.InvalidCredentials);
        }

        var token = await _jwtService.GenerateTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var response = new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Expiration = _jwtService.GetExpirationDate(),
            Roles = roles.ToList()
        };

        return Result.Success(response);
    }
}