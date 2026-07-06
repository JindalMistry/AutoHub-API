using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IPasswordHasher _passwordHasher;

    public TestController(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var hash = _passwordHasher.Hash("Password123");

        return Ok(hash);
    }
}