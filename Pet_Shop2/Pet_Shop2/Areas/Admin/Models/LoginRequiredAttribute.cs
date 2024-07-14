using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class LoginRequiredAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary{
                { "controller", "login" },
                { "action", "index" },
                { "area", "Admin" }
            });
        }

        if (!user.IsInRole("Admin"))
        {
            
        }
        else
        {
            
        }    
        // Là admin -> cho phép truy cập tài nguyên
    }
}