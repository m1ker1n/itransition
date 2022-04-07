using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using _4th_Task.Models;

namespace _4th_Task.PrincipalValidator
{
    public static class PrincipalValidator
    {
        public static async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            var userId = context.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
            
            if (userId == null)
            {
                context.RejectPrincipal();
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<UserContext>();
            var user = await dbContext.Users.FindAsync(int.Parse(userId));
            if (user == null || user.Banned)
            {
                context.RejectPrincipal();
                return;
            }
        }
    }
}
