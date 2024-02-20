using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace pbms_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file, string prefix, string suffix)
        {
            if (file == null) return BadRequest("File is null or not of type pdf");
            var fileName = DataAccess.GCP_BucketDA.UploadFile(file, prefix);
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
