using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bookShareBEnd.Database.Net
{
    public static class Helper
    {
        public static T? GetFromToken<T>(this DbSet<T> dbSet, Microsoft.AspNetCore.Http.HttpContext httpContext) where T : class
        {
            return dbSet.Find(httpContext.GetIdFromToken());
        }

        public static Guid GetIdFromToken(this Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            return Guid.Parse(httpContext.User.GetPrimarySid());
        }

        public static string? GetPrimarySid(this ClaimsPrincipal? claimsPrincipal)
        {
            return claimsPrincipal?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid")?.Value;
        }

        public static string GetRoleFromToken(this Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            return httpContext.User.GetRole();
        }

        public static string? GetRole(this ClaimsPrincipal? claimsPrincipal)
        {
            return claimsPrincipal?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
        }
    }
}
