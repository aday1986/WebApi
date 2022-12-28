using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// ChatGPT
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class ChatGPTController : Controller
    {
        private HttpClient client;

        public ChatGPTController(ConfigurationManager configuration)
        {
            var key = configuration.GetValue<string>("OpenApiKey");
            client = new HttpClient();
            client.DefaultRequestHeaders.Add($"Authorization", $"Bearer {key}");
            client.DefaultRequestHeaders.Add("OpenAI-Organization", "org-x0NYEXo7lKXltCfhaTyEvoe6");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<ApiResult<Rootobject>> Completions(string text)
        {
            var arg = new { prompt = text, model = "text-davinci-003", max_tokens = 2048 };

            HttpResponseMessage response = await client.PostAsJsonAsync("https://api.openai.com/v1/completions", arg);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Rootobject>();
                return new ApiResult<Rootobject>(result);
            }
            else
            {
                var err = await response.Content.ReadFromJsonAsync<ErrorRoot>();
                throw new Exception($"code:{err.error.code} type:{err.error.type} message:{err.error.message}");
            }
        }

        #region Model


        public class Rootobject
        {
            public string id { get; set; }
            public string _object { get; set; }
            public int created { get; set; }
            public string model { get; set; }
            public Choice[] choices { get; set; }
            public Usage usage { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }

        public class Choice
        {
            public string text { get; set; }
            public int index { get; set; }
            public object logprobs { get; set; }
            public string finish_reason { get; set; }
        }

        public class Error
        {
            /// <summary>
            /// 
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string param { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string code { get; set; }
        }

        public class ErrorRoot
        {
            /// <summary>
            /// 
            /// </summary>
            public Error error { get; set; }
        }


        #endregion
    }



}


