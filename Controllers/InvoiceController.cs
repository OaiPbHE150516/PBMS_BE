using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Material;
using pbms_be.DataAccess;
using pbms_be.ThirdParty;
using System.Net.Http.Headers;

namespace pbms_be.Controllers
{
    [Route("api/invoice")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public InvoiceController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // get all invoice by account id
        [HttpGet("get/account/{accountID}")]
        public IActionResult GetInvoices(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest("AccountID is required");
                InvoiceDA invoiceDA = new InvoiceDA(_context);
                var result = invoiceDA.GetInvoices(accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get invoice by invoice id
        [HttpGet("get/id/{invoiceID}")]
        public IActionResult GetInvoice(int invoiceID)
        {
            try
            {
                if (invoiceID <= 0) return BadRequest("InvoiceID is required");
                InvoiceDA invoiceDA = new InvoiceDA(_context);
                var result = invoiceDA.GetInvoice(invoiceID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult CreateInvoice([FromBody] string invoiceByteString)
        {
            Console.WriteLine(invoiceByteString);
            return Ok();
        }

        //[HttpPost("upload"), Route("image")]
        //public FileBundle UploadImage()
        //{

        //}

        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file, CancellationToken cancellationtoken)
        {
            //var result = await WriteFile(file);
            //return Ok(result);
            // return badrequest if file is null or not of type in the list
            if (file == null) return BadRequest("File is null or not of type pdf");
            if (file.ContentType != ConstantConfig.MINE_TYPE_PDF 
                && file.ContentType != ConstantConfig.MINE_TYPE_JPEG
                && file.ContentType != ConstantConfig.MINE_TYPE_JPG
                && file.ContentType != ConstantConfig.MINE_TYPE_PNG) 
                return BadRequest("File is null or not of type pdf, jpg or png");
            var result = DocumentAiApi.ProcessDocument(file);
            return Ok(result.Text);
        }
        //private async Task<string> WriteFile(IFormFile file)
        //{
        //    string filename = "";
        //    try
        //    {
        //        var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
        //        filename = DateTime.Now.Ticks.ToString() + extension;

        //        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files");

        //        if (!Directory.Exists(filepath))
        //        {
        //            Directory.CreateDirectory(filepath);
        //        }

        //        var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);
        //        using (var stream = new FileStream(exactpath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return filename;
        //}


        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filepath, out var contenttype))
            {
                contenttype = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
            return File(bytes, contenttype, Path.GetFileName(filepath));
        }
    }

}
