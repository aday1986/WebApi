using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using WebApi.Models;
using WebApi.Utils;

namespace WebApi.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public partial class OCRController : Controller
    {
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="serviceConfig"></param>
        public OCRController(ServiceConfig serviceConfig)
        {
            ServiceConfig = serviceConfig;
        }

        public ServiceConfig ServiceConfig { get; }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Test()
        {
            return DateTime.Now.ToString();
        }
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
            var bt = new Bitmap(file.OpenReadStream());
            foreach (var item in result.TextBlocks)
            {
                using (Graphics g = Graphics.FromImage(bt))
                {
                    g.DrawPolygon(new Pen(Brushes.Red, 2), item.BoxPoints.Select(x => new PointF() { X = x.X, Y = x.Y }).ToArray());
                }
            }
            var ms = new MemoryStream();
            bt.Save(ms, ImageFormat.Jpeg);
            var f = new FileContentResult(ms.ToArray(), file.ContentType);
            ms.Close();
            return new ApiResult<WebOCRResult>(new WebOCRResult() { File = f, Text = result.Text });
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