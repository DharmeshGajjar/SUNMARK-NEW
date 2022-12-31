using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SUNMark.Classes
{
    public class CookieHelper : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string controllerName = (string)filterContext.RouteData.Values["Controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];
            if (!controllerName.ToLower().Trim().Equals("login"))
            {


                if (!filterContext.HttpContext.Request.Cookies.TryGetValue("UserId", out string value))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "login" }, { "ReturnUrl", filterContext.HttpContext.Features.Get<IHttpRequestFeature>().RawTarget } });
                }
                else if (!filterContext.HttpContext.Request.Cookies.TryGetValue("CompanyId", out string valu))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Company" }, { "controller", "login" }, { "ReturnUrl", filterContext.HttpContext.Features.Get<IHttpRequestFeature>().RawTarget } });
                }
                else
                {
                    int cookieSessionUserId, cookieCompanyId, cookieClientId = 0, cookieYearId,isAdministrator = 0;
                    string cookieSessionUsername = string.Empty,cookieCompanyName,cookieYearName;

                    cookieSessionUserId = DbConnection.ParseInt32(filterContext.HttpContext.Request.Cookies["UserId"]);
                    cookieCompanyId = DbConnection.ParseInt32(filterContext.HttpContext.Request.Cookies["CompanyId"]);
                    cookieClientId = DbConnection.ParseInt32(filterContext.HttpContext.Request.Cookies["ClientId"]);
                    cookieSessionUsername = Convert.ToString(filterContext.HttpContext.Request.Cookies["Username"]);
                    cookieYearId = DbConnection.ParseInt32(filterContext.HttpContext.Request.Cookies["YearId"]);
                    isAdministrator = DbConnection.ParseInt32(filterContext.HttpContext.Request.Cookies["IsAdministrator"]);
                    cookieCompanyName = Convert.ToString(filterContext.HttpContext.Request.Cookies["CompanyName"]);
                    cookieYearName= Convert.ToString(filterContext.HttpContext.Request.Cookies["YearName"]);
                    

                    filterContext.HttpContext.Session.SetInt32("UserId", cookieSessionUserId);
                    filterContext.HttpContext.Session.SetString("Username", cookieSessionUsername);
                    filterContext.HttpContext.Session.SetInt32("ClientId", cookieClientId);
                    filterContext.HttpContext.Session.SetInt32("CompanyId", cookieCompanyId);
                    filterContext.HttpContext.Session.SetInt32("YearId", cookieYearId);
                    filterContext.HttpContext.Session.SetInt32("IsAdministrator", isAdministrator);
                    filterContext.HttpContext.Session.SetString("CompanyName", cookieCompanyName);
                    filterContext.HttpContext.Session.SetString("YearName", cookieYearName);
                    

                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
