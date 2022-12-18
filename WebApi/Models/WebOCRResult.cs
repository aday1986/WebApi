using Microsoft.AspNetCore.Mvc;
using PaddleOCRSharp;

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
            public List<TextBlock> TextBlocks { get; set; } = new List<TextBlock>();
            public FileContentResult File { get; set; }
        }
    }
}