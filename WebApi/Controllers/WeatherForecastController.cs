using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// ≤‚ ‘
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        /// <summary>
        /// ≤‚ ‘
        /// </summary>
        /// <param name="serviceConfig"></param>
        public TestController(ServiceConfig serviceConfig)
        {
            ServiceConfig = serviceConfig;
        }

        public ServiceConfig ServiceConfig { get; }

        /// <summary>
        /// ≤‚ ‘
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Test()
        {
            return DateTime.Now.ToString();
        }
    }
}