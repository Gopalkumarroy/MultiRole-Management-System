using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sewa360.Filters  // Match your namespace
{
    public class AdminAuthorization : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var username = session.GetString("username");  // Session set during login

            if (string.IsNullOrEmpty(username))
            {
                // Not logged in → redirect to Admin Login
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { area = "Admin", controller = "Admin", action = "Login" }
                    ));
            }
        }
    }
}
