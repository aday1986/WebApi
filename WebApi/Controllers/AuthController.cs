using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private readonly AppDbContext appDb;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtConfig"></param>
        public AuthController(JwtTokenService jwtService, AppDbContext appDb)
        {
            this.jwtService = jwtService;
            this.appDb = appDb;
        }

        /// <summary>
        /// 根据用户账号和密码获取Token。
        /// </summary>
        /// <param name="arg">账号信息</param>
        /// <remarks>例如：
        /// debug,111111
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<JwtTokenResult> Login([FromBody] LoginUserArgs arg)
        {
            var userInfo = appDb.Find<UserInfo>(arg.UserId);
            if (userInfo == null)
            {
                throw new Exception($"[{arg.UserId}] 用户账号不存在。");
            }
            if (userInfo.Pwd != MD5Encrypt64(arg.Pwd))
            {
                throw new Exception("密码错误。");
            }
            var jwtTokenResult = jwtService.GenerateEncodedToken(arg.UserId, userInfo);
            return new ApiResult<JwtTokenResult>(jwtTokenResult);
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<JwtTokenResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = jwtService.RefreshToken(refreshToken);
            return new ApiResult<JwtTokenResult>(result);
        }


        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public ApiResult<UserInfo> Register([FromBody] UserInfo userInfo)
        {
            if (userInfo == null || userInfo.UserId.Length == 0)
            {
                throw new Exception("账号不能为空。");
            }
            if (appDb.Set<UserInfo>().Find(userInfo.UserId) != null)
            {
                throw new Exception("账号已存在。");
            }
            userInfo.Pwd = MD5Encrypt64(userInfo.Pwd);
            appDb.Add(userInfo);
            appDb.SaveChanges();
            return new ApiResult<UserInfo>(userInfo);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string MD5Encrypt64(string str)
        {
            var md5 = MD5.Create();
            byte[] bt = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            return Convert.ToBase64String(bt);
        }

    }
}