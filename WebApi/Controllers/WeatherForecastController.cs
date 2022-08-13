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
    /// Ȩ��Api
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
        /// �����û��˺ź������ȡToken��
        /// </summary>
        /// <param name="arg">�˺���Ϣ</param>
        /// <remarks>���磺
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
                throw new Exception($"[{arg.userid}] �û��˺Ų����ڻ��������");
            }
            var refreshToken = Guid.NewGuid().ToString();
            var jwtTokenResult = CreateToken(arg.userid, userInfo);
            jwtTokenResult.refresh_token = refreshToken;
            return jwtTokenResult;
        }

        private JwtTokenResult CreateToken(string sub, UserInfo userInfo)
        {
            // 1. ������Ҫʹ�õ���Claims
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

            // 2. �� appsettings.json �ж�ȡSecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SecretKey));

            // 3. ѡ������㷨
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. ����Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. �������ϣ�����token
            var jwtSecurityToken = new JwtSecurityToken(
               JwtConfig.Issuer,     //Issuer
               JwtConfig.Audience,   //Audience
                claims,                          //Claims,
                DateTime.Now,                    //notBefore
                DateTime.Now.AddSeconds(30),    //expires
                signingCredentials: signingCredentials               //Credentials
            );

            // 6. ��token��Ϊstring
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
        /// ����token
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="userInfo">Я�����û���Ϣ</param>
        /// <returns></returns>
        private JwtTokenResult GenerateEncodedToken(string sub, UserInfo userInfo)
        {
            //�����û���ݱ�ʶ
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
            //��������
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
    /// �û���¼����
    /// </summary>
    public class LoginUserArgs
    {
        public string userid { get; set; }

        public string pwd { get; set; }
    }

    /// <summary>
    /// ��¼�ɹ�����model
    /// </summary>
    public class JwtTokenResult
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        /// <summary>
        /// ����ʱ��(��λ��)
        /// </summary>
        public int expires_in { get; set; }
        public string token_type { get; set; }
        //public UserInfo user { get; set; }
    }

    /// <summary>
    /// �û���Ϣ
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// �û����/����
        /// </summary>
        public string UserNo { get; set; }

        /// <summary>
        /// �û�����
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// ����Ա���ű��
        /// </summary>
        public string DepNo { get; set; }

        /// <summary>
        /// ����Ա��������
        /// </summary>
        public string DepName { get; set; }

        /// <summary>
        /// ����Ա��λ���
        /// </summary>
        public string JobNo { get; set; }

        /// <summary>
        /// ����Ա��λ����
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// �˿�
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Mac
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// ����¼ʱ��
        /// </summary>
        public DateTime OnLineTime { get; set; }

        /// <summary>
        /// ��ҵ΢��Id
        /// </summary>
        public string FWeComId { get; set; }

        /// <summary>
        /// �û���ɫ�嵥
        /// </summary>
        public List<dynamic> _MessageUserRuleList { get; set; }

        /// <summary>
        /// �û�����Ȩ���嵥
        /// </summary>
        public List<dynamic> _MessageUserDataAuthList { get; set; }
    }
}