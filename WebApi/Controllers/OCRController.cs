using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaddleOCRSharp;
using System.Drawing;
using System.Drawing.Imaging;
using WebApi.Models;
using WebApi.Utils;

namespace WebApi.Controllers
{
    /// <summary>
    /// OCR图片识别
    /// </summary>
    //[Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public partial class OCRController : Controller
    {
        /// <summary>
        /// OCR图片识别
        /// </summary>
        /// <param name="serviceConfig"></param>
        public OCRController(ServiceConfig serviceConfig)
        {
            ServiceConfig = serviceConfig;
        }

        public ServiceConfig ServiceConfig { get; }

        /// <summary>
        /// 识别图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<WebOCRResult> DetectImage(IFormFile file)
        {
            if (!file.ContentType.ToLower().StartsWith("image"))
            {
                throw new Exception("只能处理图片格式的文件。");
            }
            var bytes = StreamToBytes(file.OpenReadStream());
            var result = PaddleOCRHelper.Instance.DetectText(bytes);
            var f = ToFileContentResult(file, result);
            return new ApiResult<WebOCRResult>(new WebOCRResult() { File = f, Text = result.Text, TextBlocks=result.TextBlocks });
        }

        private FileContentResult ToFileContentResult(IFormFile file,OCRResult ocrResult)
        {
#pragma warning disable CA1416 // 验证平台兼容性
            var bt = new Bitmap(file.OpenReadStream());
            foreach (var item in ocrResult.TextBlocks)
            {
                using (Graphics g = Graphics.FromImage(bt))
                {
                    g.DrawPolygon(new Pen(Brushes.Red, 2), item.BoxPoints.Select(x => new PointF() { X = x.X, Y = x.Y }).ToArray());
                }
            }
            var ms = new MemoryStream();
            bt.Save(ms, ImageFormat.Jpeg);
#pragma warning restore CA1416 // 验证平台兼容性
            var fileResult = new FileContentResult(ms.ToArray(), file.ContentType);
            ms.Close();
            return fileResult;
        }

        /// <summary>
        /// stream转byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static byte[] StreamToBytes(Stream stream)
        {
            try
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);

                return bytes;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stream.Close();
            }
        }
    }
}