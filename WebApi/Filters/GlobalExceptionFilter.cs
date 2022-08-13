using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Models;
/// <summary>
/// 全局异常过滤器
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="loggerFactory"></param>
    public GlobalExceptionFilter(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory;
    }
    /// <summary>
    /// 
    /// </summary>
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        var logger = LoggerFactory.CreateLogger(context.ActionDescriptor.DisplayName);
        logger.LogError(context.Exception.Message);
        if (!context.ExceptionHandled)
        {
            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(new ApiResult<string>(context.Exception.Message, "执行失败", context.Exception.HResult));
        }
        context.ExceptionHandled = true;
    }

   
   
}
