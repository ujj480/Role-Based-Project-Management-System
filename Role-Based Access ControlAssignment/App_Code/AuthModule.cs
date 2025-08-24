using System;
using System.Web;

namespace App_Code
{
    public class AuthModule : IHttpModule
    {
        public void Dispose() { }
        public void Init(HttpApplication context)
        {
            context.AcquireRequestState += (s, e) =>
            {
                var ctx = HttpContext.Current;
                // Optionally enforce HTTPS or other checks
                // Could also add caching headers etc.
            };
        }
    }
}
