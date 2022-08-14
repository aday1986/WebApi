using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    /// <summary>
    /// 权限Api
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly JwtTokenService jwtService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtConfig"></param>
        public AuthController(JwtTokenService jwtService)
        {
            this.jwtService = jwtService;
        }

        /// <summary>
        /// 根据用户账号和密码获取Token。
        /// </summary>
        /// <param name="arg">账号信息</param>
        /// <remarks>例如：
        /// debug,111111
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        public ApiResult< JwtTokenResult> GetToken([FromQuery] LoginUserArgs arg)
        {
            UserInfo userInfo = null;
            if (arg.UserId == "admin" && arg.Pwd == "666")
            {
                userInfo = new UserInfo() { UserNo = "admin", UserName = "administrator" };
            }
            if (userInfo == null)
            {
                throw new Exception($"[{arg.UserId}] 用户账号不存在或密码错误。");
            }
            
            var jwtTokenResult = jwtService.GenerateEncodedToken(arg.UserId, userInfo);
            return new ApiResult<JwtTokenResult>(jwtTokenResult);
        }

    }
}