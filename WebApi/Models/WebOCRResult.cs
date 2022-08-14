using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public partial class OCRController
    {
        /// <summary>
        /// 
        /// </summary>
        public class WebOCRResult
        {
            public string Text { get; set; }

            public FileContentResult File { get; set; }
        }
    }
}