using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;

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
