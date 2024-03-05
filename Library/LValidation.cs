using pbms_be.Configurations;

namespace pbms_be.Library
{
    public class LValidation
    {
        public static bool IsCorrectPDFJPGPNG(IFormFile file)
        {
            try
            {
                var result = file.ContentType != ConstantConfig.MINE_TYPE_PDF
                               && file.ContentType != ConstantConfig.MINE_TYPE_JPEG
                               && file.ContentType != ConstantConfig.MINE_TYPE_JPG
                               && file.ContentType != ConstantConfig.MINE_TYPE_PNG;
                return !result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //IsCorrectJPGPNG
        public static bool IsCorrectJPGPNG(IFormFile file)
        {
            try
            {
                var result = file.ContentType != ConstantConfig.MINE_TYPE_JPEG
                               && file.ContentType != ConstantConfig.MINE_TYPE_JPG
                               && file.ContentType != ConstantConfig.MINE_TYPE_PNG;
                return !result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
