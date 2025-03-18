using System.ComponentModel.DataAnnotations;

namespace AuthKeycloak.Models;

public class UserUpdateModel
{
    public string? Name { get; set; }
        
    [EmailAddress]
    public string? Email { get; set; }
}