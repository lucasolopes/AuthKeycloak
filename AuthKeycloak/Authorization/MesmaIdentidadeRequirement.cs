using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthKeycloak.Authorization;

public class MesmaIdentidadeRequirement : IAuthorizationRequirement { }

public class MesmaIdentidadeHandler : AuthorizationHandler<MesmaIdentidadeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
        MesmaIdentidadeRequirement requirement)
    {
        Console.WriteLine("HandleRequirementAsync");
        if (context.Resource is HttpContext httpContext)
        {
            var userIdFromRoute = httpContext.Request.RouteValues["userId"]?.ToString();
            
            string rawToken = httpContext.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();
            
            var currentUserId = context.User.Claims
                .FirstOrDefault(c => c.Type == "sub" || 
                                  c.Type == ClaimTypes.NameIdentifier || 
                                  c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            
            if (string.IsNullOrEmpty(currentUserId) && context.User.Identity.IsAuthenticated)
            {
                var potentialIdClaim = context.User.Claims
                    .FirstOrDefault(c => c.Value == userIdFromRoute);
                    
                if (potentialIdClaim != null)
                {
                    currentUserId = potentialIdClaim.Value;
                }
            }
            
            if (!string.IsNullOrEmpty(currentUserId) && 
                (currentUserId == userIdFromRoute || context.User.IsInRole("api_admin")))
            {
                context.Succeed(requirement);
            }
        }
        
        return Task.CompletedTask;
    }
}

public class ResourceOwnershipContext
{
    public string ResourceOwnerId { get; set; }
}