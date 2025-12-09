using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SIMS.Filters
{
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth",
                    new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            if (_roles != null && _roles.Length > 0)
            {
                var userRole = context.HttpContext.Session.GetString("RoleName");

                if (!_roles.Contains(userRole))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
                }
            }
        }
    }
}