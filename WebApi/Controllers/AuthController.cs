using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// 权限Api
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtConfig"></param>
        public AuthController(IOptions<JwtConfig> jwtConfig)
        {
            JwtConfig = jwtConfig.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public JwtConfig JwtConfig { get; }

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
            var refreshToken = Guid.NewGuid().ToString();
            var jwtTokenResult = GenerateEncodedToken(arg.UserId, userInfo);
            jwtTokenResult.Refresh_Token = refreshToken;
            return new ApiResult<JwtTokenResult>(jwtTokenResult);
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="userInfo">携带的用户信息</param>
        /// <returns></returns>
        private  JwtTokenResult GenerateEncodedToken(string sub, UserInfo userInfo)
        {
            //创建用户身份标识
            var claims = new List<Claim>
            {
                new Claim("UserNo", userInfo.UserNo??string.Empty),
                new Claim("UserName", userInfo.UserName??string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, sub),
            };
            //创建令牌
            var jwt = new JwtSecurityToken(
                issuer: JwtConfig.Issuer,
                audience: JwtConfig.Audience,
                claims: claims,
                notBefore: JwtConfig.NotBefore,
                expires: JwtConfig.Expiration,
                signingCredentials: JwtConfig.SigningCredentials);

            string access_token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new JwtTokenResult()
            {
                Access_Token = access_token,
                Expires_in = JwtConfig.Expired * 60,
                Token_Type = JwtBearerDefaults.AuthenticationScheme,
                //user = userInfo
            };
        }
    }
}