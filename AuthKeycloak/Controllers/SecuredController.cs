using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SecuredController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult GetPublic()
    {
        return Ok(new { message = "Este é um endpoint público. Nenhuma autenticação necessária." });
    }

    [HttpGet("user")]
    [Authorize(Policy = "ApiUser")]
    public IActionResult GetUser()
    {
        var username = User.Identity.Name;
        var roles = User.Claims.Where(c => c.Type == "realm_access.roles" || c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        return Ok(new { message = "Endpoint protegido para usuários", username, roles });
    }

    [HttpGet("admin")]
    [Authorize(Policy = "ApiAdmin")]
    public IActionResult GetAdmin()
    {
        return Ok(new { message = "Endpoint protegido para administradores" });
    }

    [HttpGet("manager-or-admin")]
    [Authorize(Policy = "ApiManagerOrAdmin")]
    public IActionResult GetManagerOrAdmin()
    {
        return Ok(new { message = "Endpoint protegido para gerentes ou administradores" });
    }

    [HttpGet("user-info")]
    [Authorize]
    public IActionResult GetUserInfo()
    {
        // Extrai informações do token JWT
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var name = User.FindFirst("name")?.Value;

        // Busca atributos específicos das roles
        var roleAttributes = User.Claims
            .Where(c => c.Type == "role_attributes" || c.Type.Contains("attributes"))
            .Select(c => c.Value)
            .ToList();

        // Verifica permissão específica baseada em atributo de role
        var hasWritePermission = User.Claims
            .Any(c => c.Type.Contains("attributes") && c.Value.Contains("permissions") && c.Value.Contains("write"));

        return Ok(new
        {
            userId,
            email,
            name,
            roleAttributes,
            hasWritePermission,
            allClaims = claims
        });
    }
}