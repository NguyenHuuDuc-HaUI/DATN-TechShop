using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Web;

namespace MEGATECH.App_Start
{
    public static class CookieHelper
    {
        public static void Create(string name, string value, DateTime expire)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;
            cookie.Expires = expire;
            cookie.Path = "/"; // Đảm bảo cookie có hiệu lực toàn cục
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        public static string Get(string name)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                return cookie.Value;
            }
            else
            {
                return "";
            }
        }
        public static void Remove(string name)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = "";
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}