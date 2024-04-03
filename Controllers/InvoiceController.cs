using AutoMapper;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Custom;
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

        [HttpPost("scantest")]
        public IActionResult ScanInvoiceTest(IFormFile file)
        {
            if (file == null) return BadRequest(Message.FILE_IS_NULL_);
            if (LValidation.IsCorrectPDFJPGPNG(file)) return BadRequest(Message.FILE_IS_NOT_JPG_PNG);
            var result = """
                            {
                              "invoiceID": 0,
                              "transactionID": 0,
                              "supplierAddress": "Thôn Cánh Chú, Bình Yên - Thạch Thất",
                              "supplierEmail": "",
                              "supplierName": "SIÊU THỊ ĐỨC THÀNH",
                              "supplierPhone": "0865 90 9598 /",
                              "receiverAddress": "",
                              "receiverEmail": "",
                              "receiverName": "",
                              "idOfInvoice": "HDX070124-38",
                              "invoiceDate": "2024-07-01T00:00:00",
                              "invoiceType": "",
                              "paymentTerms": "",
                              "currencyID": 0,
                              "currency": null,
                              "netAmount": 0,
                              "totalAmount": "347.000 đ",
                              "taxAmount": 0,
                              "discount": 0,
                              "invoiceImageURL": "url",
                              "note": "",
                              "activeStateID": 1,
                              "activeState": null,
                              "productInInvoices": [
                                {
                                  "productID": 1,
                                  "invoiceID": 0,
                                  "productName": "",
                                  "quanity": 1,
                                  "unitPrice": 198000,
                                  "totalAmount": 198000,
                                  "note": "",
                                  "tagID": 1,
                                  "activeStateID": 0,
                                  "activeState": null
                                },
                                {
                                  "productID": 2,
                                  "invoiceID": 0,
                                  "productName": "Bánh bao nhân khoai môn Malai",
                                  "quanity": 1,
                                  "unitPrice": 26500,
                                  "totalAmount": 26500,
                                  "note": "",
                                  "tagID": 1,
                                  "activeStateID": 0,
                                  "activeState": null
                                },
                                {
                                  "productID": 3,
                                  "invoiceID": 0,
                                  "productName": "Thit san vai CP",
                                  "quanity": 1,
                                  "unitPrice": 105000,
                                  "totalAmount": 108150,
                                  "note": "",
                                  "tagID": 1,
                                  "activeStateID": 0,
                                  "activeState": null
                                },
                                {
                                  "productID": 4,
                                  "invoiceID": 0,
                                  "productName": "Phiều quà tặng (",
                                  "quanity": 1,
                                  "unitPrice": 0,
                                  "totalAmount": 0,
                                  "note": "",
                                  "tagID": 1,
                                  "activeStateID": 0,
                                  "activeState": null
                                },
                                {
                                  "productID": 5,
                                  "invoiceID": 0,
                                  "productName": "NN Sting vang pet 330ml",
                                  "quanity": 1,
                                  "unitPrice": 7000,
                                  "totalAmount": 14000,
                                  "note": "",
                                  "tagID": 1,
                                  "activeStateID": 0,
                                  "activeState": null
                                }
                              ],
                            "invoiceRawDatalog": "rawdata"
                            }
                            """;

            // wait 5 seconds to simulate processing
            System.Threading.Thread.Sleep(5000);
            return Ok(result);
            //var result = DocumentAiApi.ProcessDocument(file);
            //var imageURL = GCP_BucketDA.UploadFile(file);
            //Invoice invoice = DocumentAiApi.GetInvoiceFromDocument(result);
            // invoice.InvoiceImageURL = imageURL;
            //return Ok(invoice);
        }


        [HttpPost("scan/v2/gemini")]
        public async Task<IActionResult> ScanInvoiceTestV2(FileWithTextPrompt filescan)
        {
            var TextPromptDA = new TextPromptDA(_context);
            var textPrompt = TextPromptDA.GetTextPrompt(filescan.TextPrompt);
            if (textPrompt == null) return BadRequest("TextPrompt is not found");
            var result = await VertextAiMultimodalApi.GenerateContent(filescan.File, textPrompt);
            // remove '```json' if it exists in the result
            if (result.Contains("```json")) result = result.Replace("```json", "");
            // remove "```" if it exists in the result
            if (result.Contains("```")) result = result.Replace("```", "");
            // if first line is empty, remove it
            if (result[0] == '\n') result = result[1..];
            // remove empty lines
            result = result.Replace("\n\n", "\n");
            // remove first line
            result = result.Substring(result.IndexOf('\n') + 1);
            return Ok(result);
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
