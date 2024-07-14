using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pet_Shop2.Extensions
{
    [Route("Admin")]
    public class LoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;

            if (!isAuthenticated)
            {
                // Chưa đăng nhập thì redirect tới trang đăng nhập
                context.Result = new RedirectToRouteResult(new RouteValueDictionary{
                    { "controller", "Login" },
                    { "action", "index" }
                });
            }
            else
            {
                // Kiểm tra xem có phải admin hay không
                var isInAdminRole = context.HttpContext.User.IsInRole("Admin");

                if (!isInAdminRole)
                {
                    // Không phải admin thì trả về lỗi
                    context.Result = new UnauthorizedResult();
                }
            }

        }
    }
}
