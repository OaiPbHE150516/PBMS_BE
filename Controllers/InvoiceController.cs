using AutoMapper;
using Google.Cloud.Storage.V1;
using Google.Protobuf;
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

        //[HttpPost("scan/v2/gemini")]
        //public IActionResult ScanInvoiceV2(IFormFile file)
        //{
        //    var TextPromptDA = new TextPromptDA(_context);
        //    var textPrompt = TextPromptDA.GetTextPrompt("scan_invoice");
        //    if (textPrompt == null) return BadRequest("TextPrompt is not found");
        //    var rawData = VertextAiMultimodalApi.GenerateContent(file, textPrompt);
        //    rawData = VertextAiMultimodalApi.ProcessRawDataGemini(rawData);
        //    var result = VertextAiMultimodalApi.ProcessDataGemini(rawData);
        //    return Ok(result);
        //}

        //[HttpPost("scan/v3")]
        //public async Task<IActionResult> ScanInvoiceV3(IFormFile file)
        //{
        //    var money = await DocumentAiApi.GetMoney(file);
        //    var TextPromptDA = new TextPromptDA(_context);
        //    var textPrompt = TextPromptDA.GetTextPrompt("scan_invoice");
        //    if (textPrompt == null) return BadRequest("TextPrompt is not found");
        //    var rawData = VertextAiMultimodalApi.GenerateContent(file, textPrompt);
        //    rawData = VertextAiMultimodalApi.ProcessRawDataGemini(rawData);
        //    var result = VertextAiMultimodalApi.ProcessDataGemini(rawData);
        //    result.TotalAmount = money.TotalAmount;
        //    result.NetAmount = money.NetAmount;
        //    result.TaxAmount = money.TaxAmount;
        //    return Ok(result);
        //}


        [HttpPost("scan/v4")]
        public async Task<IActionResult> ScanInvoiceV4(IFormFile file)
        {
            //var money = await DocumentAiApi.GetMoney(file);
            var TextPromptDA = new TextPromptDA(_context);
            var textPrompt = TextPromptDA.GetTextPrompt("scan_invoice");
            if (textPrompt == null) return BadRequest("TextPrompt is not found");

            var fileByteString = ByteString.FromStream(file.OpenReadStream());
            var fileMineType = GetMimeType(file.FileName);


            Task<InvoiceCustom_VM_Scan> taskMoney = Task.Run(async () => await DocumentAiApi.GetMoney(fileByteString, fileMineType));
            Task<string> taskProduct = Task.Run(() => VertextAiMultimodalApi.GenerateContent(fileByteString, fileMineType, textPrompt));

            await Task.WhenAll(taskMoney, taskProduct);

            var invoiceByDoc = taskMoney.Result;
            var rawData = taskProduct.Result;
            rawData = VertextAiMultimodalApi.ProcessRawDataGemini(rawData);
            var invoiceByGemi = VertextAiMultimodalApi.ProcessDataGemini(rawData);
            invoiceByGemi.TotalAmount = invoiceByDoc.TotalAmount;
            invoiceByGemi.NetAmount = invoiceByDoc.NetAmount;
            invoiceByGemi.TaxAmount = invoiceByDoc.TaxAmount;
            if (string.IsNullOrEmpty(invoiceByGemi.SupplierPhone))
            {
                invoiceByGemi.SupplierPhone = invoiceByDoc.SupplierPhone;
            }
            if (string.IsNullOrEmpty(invoiceByGemi.SupplierName))
            {
                invoiceByGemi.SupplierName = invoiceByDoc.SupplierName;
            }
            if (string.IsNullOrEmpty(invoiceByGemi.SupplierAddress))
            {
                invoiceByGemi.SupplierAddress = invoiceByDoc.SupplierAddress;
            }
            // destroy 2 task if it success and return result
            taskMoney.Dispose();
            taskProduct.Dispose();

            return Ok(invoiceByGemi);
        }

        // scan/v5/custom
        [HttpPost("scan/v5/custom")]
        public async Task<IActionResult> ScanInvoiceV5Custom([FromForm] FileWithTextPrompt fileWithTextPrompt)
        {
            //var money = await DocumentAiApi.GetMoney(file);
            var TextPromptDA = new TextPromptDA(_context);
            //var textPrompt = TextPromptDA.GetTextPrompt("scan_invoice");
            //if (textPrompt == null) return BadRequest("TextPrompt is not found");

            var fileByteString = ByteString.FromStream(fileWithTextPrompt.File.OpenReadStream());
            var fileMineType = GetMimeType(fileWithTextPrompt.File.FileName);


            Task<InvoiceCustom_VM_Scan> taskMoney = Task.Run(async () => await DocumentAiApi.GetMoney(fileByteString, fileMineType));
            Task<string> taskProduct = Task.Run(() => VertextAiMultimodalApi.GenerateContent(fileByteString, fileMineType, fileWithTextPrompt.TextPrompt));

            await Task.WhenAll(taskMoney, taskProduct);

            var invoiceByDoc = taskMoney.Result;
            var rawData = taskProduct.Result;
            rawData = VertextAiMultimodalApi.ProcessRawDataGemini(rawData);
            var invoiceByGemi = VertextAiMultimodalApi.ProcessDataGemini(rawData);
            invoiceByGemi.TotalAmount = invoiceByDoc.TotalAmount;
            invoiceByGemi.NetAmount = invoiceByDoc.NetAmount;
            invoiceByGemi.TaxAmount = invoiceByDoc.TaxAmount;
            if (string.IsNullOrEmpty(invoiceByGemi.SupplierPhone))
            {
                invoiceByGemi.SupplierPhone = invoiceByDoc.SupplierPhone;
            }
            if (string.IsNullOrEmpty(invoiceByGemi.SupplierName))
            {
                invoiceByGemi.SupplierName = invoiceByDoc.SupplierName;
            }
            if (string.IsNullOrEmpty(invoiceByGemi.SupplierAddress))
            {
                invoiceByGemi.SupplierAddress = invoiceByDoc.SupplierAddress;
            }
            // destroy 2 task if it success and return result
            taskMoney.Dispose();
            taskProduct.Dispose();

            return Ok(new
            {
                rawData,
                invoiceByGemi
            });
        }

        public static string GetMimeType(string fileName)
        {
            // get mine type of file
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }



        //[HttpPost("scan/raw/v2/gemini")]
        //public IActionResult ScanInvoiceTestV2(IFormFile file)
        //{
        //    var TextPromptDA = new TextPromptDA(_context);
        //    var textPrompt = TextPromptDA.GetTextPrompt("scan_invoice");
        //    if (textPrompt == null) return BadRequest("TextPrompt is not found");
        //    var result = VertextAiMultimodalApi.GenerateContent(file, textPrompt);
        //    result = VertextAiMultimodalApi.ProcessRawDataGemini(result);
        //    return Ok(result);
        //}

        //[HttpPost("scan/v2/gemini/rawToObject")]
        //public IActionResult ScanInvoiceV2Test(string data)
        //{
        //    var result = VertextAiMultimodalApi.ProcessDataGemini(data);
        //    return Ok(result);
        //}



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
