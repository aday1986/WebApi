using PaddleOCRSharp;

namespace WebApi.Utils
{
    public static class PaddleOCRHelper
    {
        // 定义一个标识确保线程同步
        private static readonly object _Locker = new object();
        private static PaddleOCREngine _Instance;

        public static PaddleOCREngine Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_Locker)
                    {
                        // 如果类的实例不存在则创建，否则直接返回
                        if (_Instance == null)
                        {
                            //OCR参数
                            OCRParameter oCRParameter = new OCRParameter
                            {
                                numThread = 6,
                                Enable_mkldnn = 1,
                                det_db_score_mode = 0,
                                  visualize=1, 
                            };
                            OCRModelConfig config = null;
                            //服务器中英文模型
                            //OCRModelConfig config = new OCRModelConfig();
                            //string root = System.IO.Path.GetDirectoryName(typeof(OCRModelConfig).Assembly.Location);
                            //string modelPathroot = root + @"\inferenceserver";
                            //config.det_infer = modelPathroot + @"\ch_ppocr_server_v2.0_det_infer";
                            //config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
                            //config.rec_infer = modelPathroot + @"\ch_ppocr_server_v2.0_rec_infer";
                            //config.keys = modelPathroot + @"\ppocr_keys.txt";
                            _Instance = new PaddleOCREngine(config, oCRParameter);
                        }
                    }
                }
                return _Instance;
            }
        }


    }
}