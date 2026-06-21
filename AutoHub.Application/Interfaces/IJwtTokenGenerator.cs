using AutoHub.Domain.Entities;

namespace AutoHub.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
