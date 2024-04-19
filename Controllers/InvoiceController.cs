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
using pbms_be.Data.LogModel;
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
            // get current time stamp to calculate the time to process
            var startTime = DateTime.UtcNow;
            var result = """
                            {
                              "idOfInvoice": "SON230538",
                              "invoiceDate": "18/04/2024 12:00",
                              "netAmount": 208000,
                              "productInInvoices": [
                                {
                                  "productID": 1,
                                  "productName": "Giấy rút gắu trúc Sipiao 300 tở",
                                  "quanity": 1,
                                  "tag": "Giấy",
                                  "totalAmount": 5000,
                                  "unitPrice": 5000
                                },
                                {
                                  "productID": 2,
                                  "productName": "Sưả đặc ông Thợ 380g",
                                  "quanity": 1,
                                  "tag": "Sưả đặc",
                                  "totalAmount": 28000,
                                  "unitPrice": 28000
                                },
                                {
                                  "productID": 3,
                                  "productName": "Cafe G7 288g",
                                  "quanity": 1,
                                  "tag": "Cà phê",
                                  "totalAmount": 48000,
                                  "unitPrice": 48000
                                },
                                {
                                  "productID": 4,
                                  "productName": "Snickers Hạch nhân 40g",
                                  "quanity": 1,
                                  "tag": "Sóc-la",
                                  "totalAmount": 25000,
                                  "unitPrice": 25000
                                },
                                {
                                  "productID": 5,
                                  "productName": "Kẹo socola Snickers 516",
                                  "quanity": 1,
                                  "tag": "Sóc-la",
                                  "totalAmount": 22000,
                                  "unitPrice": 22000
                                },
                                {
                                  "productID": 6,
                                  "productName": "Bánh sáy bơ mật Phương Trang",
                                  "quanity": 1,
                                  "tag": "Bánh",
                                  "totalAmount": 20000,
                                  "unitPrice": 20000
                                },
                                {
                                  "productID": 7,
                                  "productName": "Redbull Thái 250ml",
                                  "quanity": 3,
                                  "tag": "Nước uống lượng",
                                  "totalAmount": 36000,
                                  "unitPrice": 12000
                                },
                                {
                                  "productID": 8,
                                  "productName": "Bánh mì Mexico Phương Trang",
                                  "quanity": 1,
                                  "tag": "Bánh",
                                  "totalAmount": 12000,
                                  "unitPrice": 12000
                                },
                                {
                                  "productID": 9,
                                  "productName": "Bánh ruốc Phương Trang",
                                  "quanity": 1,
                                  "tag": "Bánh",
                                  "totalAmount": 12000,
                                  "unitPrice": 12000
                                }
                              ],
                              "supplierAddress": "",
                              "supplierName": "Ohio Mart Thạch Hòa",
                              "supplierPhone": "0931121996",
                              "taxAmount": 0,
                              "totalAmount": 208000
                            }
                            """;

            // get random time to process
            var random = new Random();
            var randomTime = random.Next(10000, 30000);
            System.Threading.Thread.Sleep(randomTime);
            // get the end time to calculate the time to process
            var endTime = DateTime.UtcNow;
            var timeToProcess = endTime - startTime;
            // return the time to process
            return Ok(new
            {
                TimeProcess = timeToProcess,
                GeminiRawData = "Test Raw Data",
                Invoice = result
            });
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
            try
            {
                //var money = await DocumentAiApi.GetMoney(file);
                var TextPromptDA = new TextPromptDA(_context);
                var textPrompt = TextPromptDA.GetTextPrompt("scan_invoice");
                if (textPrompt == null) return BadRequest("prom is not found");

                var fileByteString = ByteString.FromStream(file.OpenReadStream());
                var fileMineType = GetMimeType(file.FileName);


                Task<InvoiceCustom_VM_Scan> taskMoney = Task.Run(async () => await DocumentAiApi.GetMoney(fileByteString, fileMineType));
                Task<string> taskProduct = Task.Run(() => VertextAiMultimodalApi.GenerateContent(fileByteString, fileMineType, textPrompt));
                Task<ScanLog> taskLog = Task.Run(() => new HandleScanLog(_context).CreateScanLog("scanV4", "MANUAL", "SUCCESS"));

                await Task.WhenAll(taskMoney, taskProduct);

                var invoiceByDoc = taskMoney.Result;
                var rawData = taskProduct.Result;
                rawData = VertextAiMultimodalApi.ProcessRawDataGemini(rawData);
                var invoiceByGemi = VertextAiMultimodalApi.ProcessDataGemini(rawData);

                if (invoiceByDoc.TotalAmount != 0) invoiceByGemi.TotalAmount = invoiceByDoc.TotalAmount;
                if (invoiceByDoc.NetAmount != 0) invoiceByGemi.NetAmount = invoiceByDoc.NetAmount;
                if (invoiceByDoc.TaxAmount != 0) invoiceByGemi.TaxAmount = invoiceByDoc.TaxAmount;

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
            catch (Exception e)
            {
                Task<ScanLog> taskLogFail = Task.Run(() => new HandleScanLog(_context).CreateScanLog("scanV4", "MANUAL", "FAIL"));
                await taskLogFail;
                return BadRequest(e.Message);
            }

        }

        [HttpPost("scan/v5")]
        public async Task<IActionResult> ScanInvoiceV5([FromForm] AccountIDWithFile file)
        {
            try
            {
                // get current time stamp to calculate the time to process
                var startTime = DateTime.UtcNow;

                var TextPromptDA = new TextPromptDA(_context);
                var textPrompt = TextPromptDA.GetTextPrompt("scan_invoice");
                if (textPrompt == null) return BadRequest("prom is not found");

                var fileByteString = ByteString.FromStream(file.File.OpenReadStream());
                var fileMineType = GetMimeType(file.File.FileName);


                Task<InvoiceCustom_VM_Scan> taskMoney = Task.Run(async () => await DocumentAiApi.GetMoney(fileByteString, fileMineType));
                Task<string> taskProduct = Task.Run(() => VertextAiMultimodalApi.GenerateContent(fileByteString, fileMineType, textPrompt));
                Task<ScanLog> taskLog = Task.Run(() => new HandleScanLog(_context).CreateScanLog(file.AccountID, "MANUAL", "SUCCESS"));

                await Task.WhenAll(taskMoney, taskProduct);

                var invoiceByDoc = taskMoney.Result;
                var rawData = taskProduct.Result;
                rawData = VertextAiMultimodalApi.ProcessRawDataGemini(rawData);
                var invoiceByGemi = VertextAiMultimodalApi.ProcessDataGemini(rawData);

                if (invoiceByDoc.TotalAmount != 0) invoiceByGemi.TotalAmount = invoiceByDoc.TotalAmount;
                if (invoiceByDoc.NetAmount != 0) invoiceByGemi.NetAmount = invoiceByDoc.NetAmount;
                if (invoiceByDoc.TaxAmount != 0) invoiceByGemi.TaxAmount = invoiceByDoc.TaxAmount;

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

                // get the end time to calculate the time to process
                var endTime = DateTime.UtcNow;
                var timeToProcess = endTime - startTime;
                // return the time to process
                return Ok(new
                {
                    TimeProcess = timeToProcess,
                    GeminiRawData = rawData,
                    Invoice = invoiceByGemi
                });
            }
            catch (Exception e)
            {
                Task<ScanLog> taskLogFail = Task.Run(() => new HandleScanLog(_context).CreateScanLog(file.AccountID, "MANUAL", "FAIL"));
                await taskLogFail;
                return BadRequest(e.Message);
            }
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
