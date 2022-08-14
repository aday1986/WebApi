using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Models;

namespace WebApi.Services
{
    /// <summary>
    /// JwtToken服务
    /// </summary>
    public class JwtTokenService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtConfig"></param>
        public JwtTokenService(IOptions<JwtConfig> jwtConfig)
        {
            JwtConfig = jwtConfig.Value;
        }

        /// <summary>
        /// 配置文件
        /// </summary>
        public JwtConfig JwtConfig { get; }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="userInfo">携带的用户信息</param>
        /// <returns></returns>
        public JwtTokenResult GenerateEncodedToken(string sub, UserInfo userInfo)
        {
            //创建用户身份标识
            var claims = new List<Claim>
            {
                new Claim(nameof(UserInfo.UserNo), userInfo.UserNo??string.Empty),
                new Claim(nameof(UserInfo.UserName), userInfo.UserName??string.Empty),
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
            var refreshToken = Guid.NewGuid().ToString();
            return new JwtTokenResult()
            {
                Access_Token = access_token,
                Expires_in = JwtConfig.Expired * 60,
                Token_Type = JwtBearerDefaults.AuthenticationScheme,
                Refresh_Token = refreshToken,
                UserInfo = userInfo
            };
        }
    }
}
