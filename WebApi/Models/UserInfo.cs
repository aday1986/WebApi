namespace WebApi.Models
{
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


    }
}