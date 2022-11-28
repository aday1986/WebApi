namespace WebApi.Models
{
    /// <summary>
    /// 登录成功返回model
    /// </summary>
    public class JwtTokenResult
    {
        /// <summary>
        /// token
        /// </summary>
        public string Access_Token { get; set; }

        /// <summary>
        /// 刷新token
        /// </summary>
        public string Refresh_Token { get; set; }
        /// <summary>
        /// 过期时间(单位秒)
        /// </summary>
        public int Expires_in { get; set; }

        /// <summary>
        /// token 类型
        /// </summary>
        public string Token_Type { get; set; }


    }
}