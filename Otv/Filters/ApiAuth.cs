using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace DanMu.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ApiAuth : ActionFilterAttribute
    {
        private string[] auths = { "member"};
        public string Auth { get; set; }
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            bool isAccess = false;
            try
            {
                if (actionContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0)   // 允许匿名访问
                {
                    base.OnActionExecuting(actionContext);
                    return;
                }

                string[] authArgs = Auth.Split(',');
                foreach (string r in authArgs)
                {
                    if (auths.Contains(r))//验证通过
                    {
                        isAccess = true;
                        break;
                    }
                }

                if (!isAccess)
                {
                    actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Message", action = "Index" }));
                    return;
                }

                // TODO: 添加其它验证方法

                base.OnActionExecuting(actionContext);
            }
            catch
            {
                actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Message", action = "Index" }));
            }
        }
    }
}