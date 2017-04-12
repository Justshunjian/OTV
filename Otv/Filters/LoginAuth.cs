using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DanMu.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class LoginAuth : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            if (httpContext.User.Identity.IsAuthenticated && base.AuthorizeCore(httpContext))
            {
                return ValidateUser();
            }

            if (httpContext.Session["User"] != null)
            {
                return ValidateUser();
            }

            httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                filterContext.HttpContext.Response.Redirect(FormsAuthentication.LoginUrl);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Response.Redirect(FormsAuthentication.LoginUrl);
        }

        private bool ValidateUser()
        {
            //TODO: 权限验证
            return true;
        }
    }
}