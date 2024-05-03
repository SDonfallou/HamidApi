using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace bookShareBEnd.Database.Net
{
    public static class Helper
    {
        //public static T? GetFromToken<T>(this DbSet<T> dbSet, Microsoft.AspNetCore.Http.HttpContext httpContext) where T : class
        //{
        //    return dbSet.Find(httpContext.GetIdFromToken());
        //}

        //public static Guid GetIdFromToken(this Microsoft.AspNetCore.Http.HttpContext httpContext)
        //{
        //    return Guid.Parse(httpContext.User.GetNameIdentifier());
        //}

        //public static string? GetNameIdentifier(this ClaimsPrincipal? claimsPrincipal)
        //{
        //    return claimsPrincipal?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid")?.Value;
        //}

        //public static string GetRoleFromToken(this Microsoft.AspNetCore.Http.HttpContext httpContext)
        //{
        //    return httpContext.User.GetRole();
        //}

        //public static string? GetRole(this ClaimsPrincipal? claimsPrincipal)
        //{
        //    return claimsPrincipal?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
        //}

    
    }
    public static class HttpContextUtils
    {
        public static Guid? GetIdFromToken(this HttpContext httpContext)
        {
            var primarySid = httpContext.User.GetNameIdentifier(); 

            if (!string.IsNullOrEmpty(primarySid))
            {
                if (Guid.TryParse(primarySid, out Guid id)) 
                {
                    return id; 
                }
                else
                {
                    // Log or handle the case where PrimarySid cannot be parsed into a GUID
                }
            }

            return null; 
        }

        public static string? GetNameIdentifier(this ClaimsPrincipal? claimsPrincipal)
        {
            return claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }



}
