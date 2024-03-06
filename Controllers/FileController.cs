using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.DataAccess;
using pbms_be.Library;

namespace pbms_be.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file, string prefix, string suffix, string folder)
        {
            // only accept image, pdf, doc, docx, xls, xlsx, ppt, pptx, txt
            if (file == null) return BadRequest("File is null");
            var mineType = file.ContentType;
            if (mineType != ConstantConfig.MINE_TYPE_PDF && mineType != ConstantConfig.MINE_TYPE_JPEG && mineType != ConstantConfig.MINE_TYPE_PNG)
                return BadRequest(Message.FILE_IS_NOT_PDF_JPG_PNG);
            // 
            var fileName = DataAccess.GCP_BucketDA.UploadFile(file, prefix, folder);
            return Ok(fileName);
        }

        [HttpPost("upload/collabfund/imagecover")]
        public IActionResult UploadCollabFundImageCover(IFormFile file)
        {
            // only accept image, pdf, doc, docx, xls, xlsx, ppt, pptx, txt
            if (file is null) return BadRequest(Message.FILE_IS_NULL_);
            var mineType = file.ContentType;
            if (mineType != ConstantConfig.MINE_TYPE_JPEG
                && mineType != ConstantConfig.MINE_TYPE_PNG
                && mineType != ConstantConfig.MINE_TYPE_JPG)
                return BadRequest(Message.FILE_IS_NOT_JPG_PNG);
            var filename = LConvertVariable.GenerateRandomString(CloudStorageConfig.DEFAULT_FILE_NAME_LENGTH, Path.GetFileNameWithoutExtension(file.FileName));
            var fileURL = GCP_BucketDA.UploadFileCustom(file, CloudStorageConfig.PBMS_BUCKET_NAME, CloudStorageConfig.COLLAB_FUND_FOLDER,
                                                        "imagecover", filename, "file", true);
            return Ok(fileURL);
        }

        // upload invoice file
        [HttpPost("upload/transaction/invoice")]
        public IActionResult UploadInvoiceFile(IFormFile file)
        {
            // only accept image, pdf, doc, docx, xls, xlsx, ppt, pptx, txt
            if (file is null) return BadRequest(Message.FILE_IS_NULL_);
            if (LValidation.IsCorrectPDFJPGPNG(file)) return BadRequest(Message.FILE_IS_NOT_JPG_PNG);
            var filename = LConvertVariable.GenerateRandomString(CloudStorageConfig.DEFAULT_FILE_NAME_LENGTH, Path.GetFileNameWithoutExtension(file.FileName));
            var fileURL = GCP_BucketDA.UploadFileCustom(file, CloudStorageConfig.PBMS_BUCKET_NAME, CloudStorageConfig.INVOICE_FOLDER,
                                                        "invoice", filename, "file", true);
            return Ok(fileURL);
        }

        //// download file
        //[HttpGet("download/{fileName}")]
        //public IActionResult DownloadFile(string fileName)
        //{
        //    if (string.IsNullOrEmpty(fileName)) return BadRequest("File name is required");
        //    var file = DataAccess.GCP_BucketDA.DownloadFile(fileName);
        //    if (file == null) return BadRequest("File not found");
        //    return File(file, "application/pdf", fileName);
        //}
    }
}
