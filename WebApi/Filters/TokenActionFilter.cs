using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
/// <summary>
/// 用于获取用户信息
/// </summary>
public class TokenActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var test = context.HttpContext.Request.Path;
        string bearer = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (bearer == null || bearer.Length <= 6 || bearer.Substring(0, 6).ToLower() != "bearer")
        {
            return;
        }
        string[] jwt = bearer.Split(' ');
        var tokenObj = new JwtSecurityToken(jwt[1]);

        var claimsIdentity = new ClaimsIdentity(tokenObj.Claims);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        context.HttpContext.User = claimsPrincipal;
    }
}
