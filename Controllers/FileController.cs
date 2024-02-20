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
            var fileName = DataAccess.GCP_BucketDA.UploadFile(file, prefix, suffix);
            return Ok(fileName);
        }
    }
}
