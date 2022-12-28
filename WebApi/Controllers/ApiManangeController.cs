using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiManangeController : Controller
    {
        public ApiManangeController(ApplicationPartManager manager, ICompiler compiler, DynamicChangeTokenProvider tokenProvider)
        {
            Manager = manager;
            Compiler = compiler;
            TokenProvider = tokenProvider;
        }

        private ApplicationPartManager Manager { get; }
        private ICompiler Compiler { get; }
        private DynamicChangeTokenProvider TokenProvider { get; }

        [HttpPost]
        public ApiResult<string> Add(string source)
        {
            source = @$"
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;

" + source;
            Manager.ApplicationParts.Add(new AssemblyPart(Compiler.Compile(source, Assembly.Load(new AssemblyName("System.Runtime")),
                typeof(object).Assembly,
                typeof(ControllerBase).Assembly,
                typeof(Controller).Assembly)));
            TokenProvider.NotifyChanges();
            return new ApiResult<string>("");
        }
    }
}

//[Route("api/[controller]/[action]")]
//[ApiController]
//public class TestController : Controller
//{
//    public string Add()
//    {
//        return "执行成功。";
//    }
//}
