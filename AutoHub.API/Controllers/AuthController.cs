using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Auth;
using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
    RegisterRequest request)
    {
        await _authService.RegisterAsync(request);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "User registered successfully.",
            Data = null
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
    LoginRequest request)
    {
        var response =
            await _authService.LoginAsync(request);

        return Ok(new ApiResponse<AuthResponse>
        {
            Success = true,
            Message = "Login successful.",
            Data = response
        });
    }
}
