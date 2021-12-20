using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartDoor.Server.Models.Database;

namespace SmartDoor.Server.Middleware;

public class AuthorizationFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var attribs = (context.Controller as Controller)?.ControllerContext?.ActionDescriptor?.EndpointMetadata?.Where(x => x is AccessRoleAttribute).Select(x => (AccessRoleAttribute)x);
        var role = context.HttpContext.GetUser()?.Role ?? UserRole.Anonymous;
        
        if (attribs is not null)
            if (!attribs.Any(a => role.HasFlag(a.Role)))
                context.Result = new ContentResult() { StatusCode = role == UserRole.Anonymous ? 401 : 403 };
    }
}