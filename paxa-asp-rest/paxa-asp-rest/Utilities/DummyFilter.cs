using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

// To be used when running local test without Google authentication
namespace paxa.Utilities
{
    public class DummyFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            filterContext.Request.Properties.Add("ProfileId", "123456");
            filterContext.Request.Properties.Add("ProfileName", "Kalle");
            filterContext.Request.Properties.Add("ProfileEmail", "kalle@kalle.com");
            base.OnActionExecuting(filterContext);
        }
    }
}