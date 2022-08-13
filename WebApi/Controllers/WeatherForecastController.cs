using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;
using WebApi.Utils;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        public TestController()
        {
        }

        [HttpGet]
        public string Test()
        {
            return DateTime.Now.ToString();
        }
    }

    /// <summary>
    /// 权限Api
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {
        public AuthController(IOptions<JwtConfig> jwtConfig)
        {
            JwtConfig = jwtConfig.Value;
        }

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
        public JwtTokenResult GetToken([FromQuery] LoginUserArgs arg)
        {
            UserInfo userInfo = null;
            if (arg.userid == "admin" && arg.pwd == "666")
            {
                userInfo = new UserInfo() { UserNo = "admin" , UserName="administrator"};
            }
            if (userInfo == null)
            {
                throw new Exception($"[{arg.userid}] 用户账号不存在或密码错误。");
            }
            var refreshToken = Guid.NewGuid().ToString();
            var jwtTokenResult = CreateToken(arg.userid, userInfo);
            jwtTokenResult.refresh_token = refreshToken;
            return jwtTokenResult;
        }

        private JwtTokenResult CreateToken(string sub, UserInfo userInfo)
        {
            // 1. 定义需要使用到的Claims
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, "u_admin"), //HttpContext.User.Identity.Name
            new Claim(ClaimTypes.Role, "r_admin"), //HttpContext.User.IsInRole("r_admin")
            new Claim(JwtRegisteredClaimNames.Jti, "admin"),
            new Claim("UserNo", userInfo.UserNo),
            new Claim("UserName", userInfo.UserName),
            //new Claim("Permission", Permissions.UserCreate),
            //new Claim("Permission", Permissions.UserUpdate),
            new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddSeconds(30).ToString()),
        };

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SecretKey));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
               JwtConfig.Issuer,     //Issuer
               JwtConfig.Audience,   //Audience
                claims,                          //Claims,
                DateTime.Now,                    //notBefore
                DateTime.Now.AddSeconds(30),    //expires
                signingCredentials: signingCredentials               //Credentials
            );

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return new JwtTokenResult()
            {
                access_token = token,
                expires_in = JwtConfig.Expired * 60,
                token_type = JwtBearerDefaults.AuthenticationScheme,
                //user = userInfo
            };
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="userInfo">携带的用户信息</param>
        /// <returns></returns>
        private JwtTokenResult GenerateEncodedToken(string sub, UserInfo userInfo)
        {
            //创建用户身份标识
            var claims = new List<Claim>
            {
                new Claim("UserNo", userInfo.UserNo??string.Empty),
                new Claim("UserName", userInfo.UserName??string.Empty),
                 new Claim("DepNo", userInfo.DepNo??string.Empty),
                new Claim("DepName", userInfo.DepName??string.Empty),
                 new Claim("JobNo", userInfo.JobNo??string.Empty),
                new Claim("JobName", userInfo.JobName??string.Empty),
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
                access_token = access_token,
                expires_in = JwtConfig.Expired * 60,
                token_type = JwtBearerDefaults.AuthenticationScheme,
                //user = userInfo
            };
        }
    }

    /// <summary>
    /// 用户登录参数
    /// </summary>
    public class LoginUserArgs
    {
        public string userid { get; set; }

        public string pwd { get; set; }
    }

    /// <summary>
    /// 登录成功返回model
    /// </summary>
    public class JwtTokenResult
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        /// <summary>
        /// 过期时间(单位秒)
        /// </summary>
        public int expires_in { get; set; }
        public string token_type { get; set; }
        //public UserInfo user { get; set; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户编号/工号
        /// </summary>
        public string UserNo { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 操作员部门编号
        /// </summary>
        public string DepNo { get; set; }

        /// <summary>
        /// 操作员部门名称
        /// </summary>
        public string DepName { get; set; }

        /// <summary>
        /// 操作员岗位编号
        /// </summary>
        public string JobNo { get; set; }

        /// <summary>
        /// 操作员岗位名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Mac
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime OnLineTime { get; set; }

        /// <summary>
        /// 企业微信Id
        /// </summary>
        public string FWeComId { get; set; }

        /// <summary>
        /// 用户角色清单
        /// </summary>
        public List<dynamic> _MessageUserRuleList { get; set; }

        /// <summary>
        /// 用户数据权限清单
        /// </summary>
        public List<dynamic> _MessageUserDataAuthList { get; set; }
    }
}