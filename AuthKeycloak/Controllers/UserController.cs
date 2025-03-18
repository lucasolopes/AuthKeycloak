using AuthKeycloak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthKeycloak.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{userId}")]
    [Authorize(Policy = "SomenteProprioUsuario")]
    public IActionResult GetUserProfile(string userId)
    {
        return Ok(new { message = "Perfil do usuário obtido com sucesso" });
    }
    
    [HttpPut("{userId}")]
    [Authorize(Policy = "SomenteProprioUsuario")]
    public IActionResult UpdateUserProfile(string userId, UserUpdateModel model)
    {
        return Ok(new { message = "Perfil atualizado com sucesso" });
    }
}