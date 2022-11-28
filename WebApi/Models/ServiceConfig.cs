namespace WebApi.Models
{
    /// <summary>
    /// 接收消息体
    /// </summary>
    public class ServiceConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        public ServiceConfig(IHttpContextAccessor httpContext)
        {
            var user = httpContext?.HttpContext?.User;
            if (user == null)
            {
                User = new UserInfo();
            }
            else
            {
                User = new UserInfo()
                {
                    UserId = user.FindFirst("UserId")?.Value,
                    UserName = user.FindFirst("UserName")?.Value,
                };
            }
           
        }

        /// <summary>
        /// 帐号登录信息
        /// </summary>
        public UserInfo User { get; set; }

    }
}
