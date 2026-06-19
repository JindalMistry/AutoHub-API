using AutoHub.Application.DTOs.Auth;
using AutoHub.Application.Exceptions;
using AutoHub.Application.Interfaces;
using AutoHub.Domain.Entities;
using AutoHub.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace AutoHub.Infrastructure.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly ApplicationDbcontext _dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public AuthService(IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher, ApplicationDbcontext dbcontext)
    {
        _passwordHasher = passwordHasher;
        _dbContext = dbcontext;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> LoginAsync(
    LoginRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            throw new NotFoundException("User does not exist!");
        }

        var isValidPassword = _passwordHasher.Verify(
            request.Password,
            user.PasswordHash);

        if (!isValidPassword)
        {
            throw new BadRequestException("Invalid Password!");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(o => o.Email == request.Email);

        if (existingUser != null)
        {
            throw new BadRequestException("Email already exists");
        }

        var hashPassword = _passwordHasher.Hash(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = hashPassword,
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();
    }
}

