namespace WebApi.Models
{
    /// <summary>
    /// 用户登录参数
    /// </summary>
    public class LoginUserArgs
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
    }
}