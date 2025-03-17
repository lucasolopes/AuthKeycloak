using AuthKeycloak.Extensions;
using AuthKeycloak.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthKeycloak.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var tokenResponse = await _tokenService.GetTokenAsync(request.Username, request.Password);
            return Ok(tokenResponse);
        }
        catch (HttpRequestException ex)
        {
            return Unauthorized("Credenciais inválidas");
        }
    }

    [HttpPost("client-credentials")]
    public async Task<IActionResult> ClientCredentials()
    {
        try
        {
            var tokenResponse = await _tokenService.GetClientCredentialsTokenAsync();
            return Ok(tokenResponse);
        }
        catch (HttpRequestException ex)
        {
            return BadRequest("Erro ao obter token: " + ex.Message);
        }
    }
}