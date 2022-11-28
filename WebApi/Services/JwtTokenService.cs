using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        /// 用于Token刷新，待实现
        /// </summary>
        private static ConcurrentDictionary<string, UserInfo> TokenCache { get; } = new ConcurrentDictionary<string, UserInfo>();

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
                new Claim(nameof(UserInfo.UserId), userInfo.UserId??string.Empty),
                new Claim(nameof(UserInfo.UserName), userInfo.UserName??string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, sub),
            };
            var jwt = new JwtSecurityToken(
                issuer: JwtConfig.Issuer,
                audience: JwtConfig.Audience,
                claims: claims,
                notBefore: JwtConfig.NotBefore,
                expires: JwtConfig.Expiration,
                signingCredentials: JwtConfig.SigningCredentials);
            string access_token = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = Guid.NewGuid().ToString();
            var result = new JwtTokenResult()
            {
                Access_Token = access_token,
                Expires_in = JwtConfig.Expired * 60,
                Token_Type = JwtBearerDefaults.AuthenticationScheme,
                Refresh_Token = refreshToken,
            };
            if (TokenCache.TryAdd(result.Refresh_Token, userInfo))
            {
                return result;
            }
            else
            {
                throw new Exception("生成Token失败。");
            }
        }


        /// <summary>
        /// 根据refresh_Token刷新JwtToken
        /// </summary>
        /// <param name="refresh_Token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public JwtTokenResult RefreshToken(string refresh_Token)
        {
            if (TokenCache.TryGetValue(refresh_Token, out var userInfo))
            {
                return GenerateEncodedToken(userInfo.UserId, userInfo);
            }
            else
            {
                throw new Exception("Token刷新失败。");
            }
        }
    }
}
