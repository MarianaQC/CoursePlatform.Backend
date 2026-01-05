using CoursePlatform.Application.Common;
using CoursePlatform.Application.DTOs.Auth;
using CoursePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<AuthResponse>.Fail(result.Error));
        }

        return Ok(ApiResponse<AuthResponse>.Ok(result.Value, "Usuario registrado exitosamente."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(ApiResponse<AuthResponse>.Fail(result.Error));
        }

        return Ok(ApiResponse<AuthResponse>.Ok(result.Value, "Inicio de sesión exitoso."));
    }
}