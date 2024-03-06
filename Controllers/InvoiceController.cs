using AutoMapper;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Invo;
using pbms_be.Data.Material;
using pbms_be.DataAccess;
using pbms_be.Library;
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
        private InvoiceDA _invoiceDA;

        public InvoiceController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _invoiceDA = new InvoiceDA(context);
        }

        // scanning invoice
        [HttpPost("scan")]
        public IActionResult ScanInvoice(IFormFile file)
        {
            if (file == null) return BadRequest(Message.FILE_IS_NULL_);
            if (LValidation.IsCorrectPDFJPGPNG(file)) return BadRequest(Message.FILE_IS_NOT_JPG_PNG);
            var result = DocumentAiApi.ProcessDocument(file);
            //var imageURL = GCP_BucketDA.UploadFile(file);
            Invoice invoice = DocumentAiApi.GetInvoiceFromDocument(result);
           // invoice.InvoiceImageURL = imageURL;
            return Ok(invoice);
        }


        // get all invoice by account id
        //[HttpGet("get/account/{accountID}")]
        //public IActionResult GetInvoices(string accountID)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(accountID)) return BadRequest("AccountID is required");
        //        InvoiceDA invoiceDA = new InvoiceDA(_context);
        //        var result = invoiceDA.GetInvoices(accountID);
        //        return Ok(result);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        // get invoice by invoice id
        //[HttpGet("get/id/{invoiceID}")]
        //public IActionResult GetInvoice(int invoiceID)
        //{
        //    try
        //    {
        //        if (invoiceID <= 0) return BadRequest("InvoiceID is required");
        //        InvoiceDA invoiceDA = new InvoiceDA(_context);
        //        var result = invoiceDA.GetInvoice(invoiceID);
        //        return Ok(result);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //[HttpPost("create")]
        //public IActionResult CreateInvoice([FromBody] string invoiceByteString)
        //{
        //    Console.WriteLine(invoiceByteString);
        //    return Ok();
        //}

        //[HttpPost("upload"), Route("image")]
        //public FileBundle UploadImage()
        //{

        //}

        //[HttpPost("upload")]
        //public IActionResult UploadFile(IFormFile file, CancellationToken cancellationtoken)
        //{
        //    if (file == null) return BadRequest("File is null or not of type pdf");
        //    if (file.ContentType != ConstantConfig.MINE_TYPE_PDF
        //        && file.ContentType != ConstantConfig.MINE_TYPE_JPEG
        //        && file.ContentType != ConstantConfig.MINE_TYPE_JPG
        //        && file.ContentType != ConstantConfig.MINE_TYPE_PNG)
        //        return BadRequest("File is null or not of type pdf, jpg or png");
        //    var result = DocumentAiApi.ProcessDocument(file);
        //    var imageURL = GCP_BucketDA.UploadFile(file);
        //    Invoice invoice = DocumentAiApi.GetInvoiceFromDocument(result);
        //    invoice.InvoiceImageURL = imageURL;
        //    return Ok(invoice);
        //}

        //[HttpGet]
        //[Route("DownloadFile")]
        //public async Task<IActionResult> DownloadFile(string filename)
        //{
        //    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);

        //    var provider = new FileExtensionContentTypeProvider();
        //    if (!provider.TryGetContentType(filepath, out var contenttype))
        //    {
        //        contenttype = "application/octet-stream";
        //    }

        //    var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
        //    return File(bytes, contenttype, Path.GetFileName(filepath));
        //}
    }

}
