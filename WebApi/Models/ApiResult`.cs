namespace WebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public ApiResult( T data, string message="执行成功", int status=200)
        {
            Status = status;
            Message = message;
            Data = data;
        }


        /// <summary>
        /// 返回代码
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// /返回数据
        /// </summary>
        public T Data { get; set; }
    }

}
