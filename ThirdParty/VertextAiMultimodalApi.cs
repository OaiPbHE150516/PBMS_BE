using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;
using Google.Protobuf;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pbms_be.Configurations;
using pbms_be.Data.Custom;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace pbms_be.ThirdParty
{
    public class VertextAiMultimodalApi
    {
        public static string GenerateContent(ByteString fileByteString, string fileMineType, string textprompt)
        {
            string projectId = ConstantConfig.PROJECT_ID;
            string location = "us-central1";
            string publisher = "google";
            string model = "gemini-1.0-pro-vision";


            // Create client
            var predictionServiceClient = new PredictionServiceClientBuilder
            {
                Endpoint = $"{location}-aiplatform.googleapis.com"
            }.Build();

            // Images
            //ByteString colosseum = await ReadImageFileAsync(
            //    "https://storage.googleapis.com/pbms-user/invoice/2024-03-09%2016.24.53.jpeg");

            // Initialize request argument(s)
            var content = new Content
            {
                Role = "USER"
            };
            content.Parts.AddRange(new List<Part>()
            {
                new()
                {
                    InlineData = new()
                    {
                        MimeType = fileMineType,
                        Data = fileByteString

                    }
                },
                new()
                {
                    Text = textprompt
                },
            });

            var generateContentRequest = new GenerateContentRequest
            {
                Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}"
            };
            generateContentRequest.Contents.Add(content);
            GenerateContentResponse response = predictionServiceClient.GenerateContent(generateContentRequest);
            return response.Candidates[0].Content.Parts[0].Text;
        }

        private static async Task<ByteString> ReadImageFileAsync(string url)
        {
            using HttpClient client = new();
            using var response = await client.GetAsync(url);
            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            return ByteString.CopyFrom(imageBytes);
        }

        internal static string ProcessRawDataGemini(string rawData)
        {
            var result = rawData;
            if (result.Contains("```json")) result = result.Replace("```json", "");
            if (result.Contains("```")) result = result.Replace("```", "");
            if (result.Contains("json")) result = result.Replace("json", "");
            if (result[0] == '\n') result = result[1..];
            result = result.Replace("\n\n", "\n");
            result = result[(result.IndexOf('\n') + 1)..];
            return result;
        }

        internal static InvoiceCustom_VM_Scan ProcessDataGemini(string data)
        {
            try
            {
                if (data is null) throw new Exception("Data is null");
                JObject? jsonOject = JsonConvert.DeserializeObject<JObject>(data) ?? throw new Exception("Json object is null");
                var idOfInvoiceRaw = jsonOject["invoice_id"]?.ToString() ?? "";
                var invoiceDateRaw = jsonOject["invoice_date"]?.ToString() ?? "";
                var supplierNameRaw = jsonOject["supplierName"]?.ToString() ?? "";
                var supplierAddressRaw = jsonOject["supplierAddress"]?.ToString() ?? "";
                var supplierPhoneRaw = jsonOject["supplierPhone"]?.ToString() ?? "";
                var totalAmountRaw = jsonOject["totalAmount"]?.ToString() ?? "";
                var netAmountRaw = jsonOject["netAmount"]?.ToString() ?? "";
                var taxAmountRaw = jsonOject["taxAmount"]?.ToString() ?? "";

                var invoiceResult = new InvoiceCustom_VM_Scan
                {
                    IDOfInvoice = idOfInvoiceRaw,
                    InvoiceDate = invoiceDateRaw,
                    SupplierName = supplierNameRaw,
                    SupplierAddress = supplierAddressRaw,
                    SupplierPhone = supplierPhoneRaw,

                    TotalAmount = long.TryParse(totalAmountRaw, out var totalAmountResultInvoice) ? totalAmountResultInvoice : 0,
                    NetAmount = long.TryParse(netAmountRaw, out var netAmountResult) ? netAmountResult : 0,
                    TaxAmount = long.TryParse(taxAmountRaw, out var taxAmountResult) ? taxAmountResult : 0
                };

                var productsData = new List<ProductInInvoice_VM_Scan>();
                var productsRaw = jsonOject["line_item"]?.ToObject<List<JObject>>();
                if (productsRaw is null) return invoiceResult;
                var countProduct = 1;
                foreach (var product in productsRaw)
                {
                    var productName = product["line_item/description"]?.ToString() ?? "";
                    var quantity = product["line_item/quantity"]?.ToString() ?? "";
                    var unitPrice = product["line_item/unitPrice"]?.ToString() ?? "";
                    var totalAmount = product["line_item/amount"]?.ToString() ?? "";
                    var tag = product["line_item/tag"]?.ToString() ?? "";

                    var productResult = new ProductInInvoice_VM_Scan
                    {
                        ProductID = countProduct,
                        ProductName = productName,
                        Quanity = int.TryParse(quantity, out var quantityResult) ? quantityResult : 0,
                        UnitPrice = long.TryParse(unitPrice, out var unitPriceResult) ? unitPriceResult : 0,
                        TotalAmount = long.TryParse(totalAmount, out var totalAmountResultProduct) ? totalAmountResultProduct : 0,
                        Tag = tag
                    };
                    productsData.Add(productResult);
                    countProduct++;
                }
                invoiceResult.ProductInInvoices = productsData;
                return invoiceResult;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // function to get mime type of a file
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

        // classify tag by using Vertext AI Multimodal API
        public static string ClassifyTag(string text_prompt, string data)
        {
            string projectId = ConstantConfig.PROJECT_ID;
            string location = "us-central1";
            string publisher = "google";
            string model = "gemini-1.0-pro-vision";


            // Create client
            var predictionServiceClient = new PredictionServiceClientBuilder
            {
                Endpoint = $"{location}-aiplatform.googleapis.com"
            }.Build();

            // Initialize request argument(s)
            var content = new Content
            {
                Role = "USER"
            };
            content.Parts.AddRange(new List<Part>()
            {
                new()
                {
                    Text = text_prompt
                },
                new()
                {
                    Text = data
                },
            });

            var generateContentRequest = new GenerateContentRequest
            {
                Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}"
            };
            generateContentRequest.Contents.Add(content);
            GenerateContentResponse response = predictionServiceClient.GenerateContent(generateContentRequest);
            return ProcessRawDataGemini(response.Candidates[0].Content.Parts[0].Text);
        }
    }
}
