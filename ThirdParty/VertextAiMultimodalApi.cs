using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;
using Google.Protobuf;
using pbms_be.Configurations;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace pbms_be.ThirdParty
{
    public class VertextAiMultimodalApi
    {
        public static async Task<string> GenerateContent(IFormFile file)
        {
            string projectId = "lexical-aileron-410114";
            string location = "asia-southeast1";
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

            //ByteString forbiddenCity = await ReadImageFileAsync(
            //    "https://storage.googleapis.com/cloud-samples-data/vertex-ai/llm/prompts/landmark2.png");

            //ByteString christRedeemer = await ReadImageFileAsync(
            //    "https://storage.googleapis.com/cloud-samples-data/vertex-ai/llm/prompts/landmark3.png");

            ByteString colosseum = ByteString.FromStream(file.OpenReadStream());
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
                        MimeType = "image/jpeg",
                        Data = colosseum

                    }
                },
                new()
                {
                    Text = "Extract the name, phone number, address of the sales unit, id number and date of the order, item name, quantity, unit price and total price of the items with it tag - which is kind of product, total amount of the invoice from the invoice image and export them as JSON with the key as below:\r\n{\r\n\"supplierAddress\": \"\",\r\n\"supplierName\": \"\",\r\n\"supplierPhone\": \"\",\r\n\"invoice_date\": \"\",\r\n\"invoice_id\": \"\",\r\n\"netAmount\": 0,\r\n\"totalAmount\": 0,\r\n\"taxAmount\": 0,\r\n\"line_item\": [\r\n    {\r\n      \"line_item/description\": \"\",\r\n      \"line_item/quanity\": 1,\r\n      \"line_item/unitPrice\": 0,\r\n      \"line_item/amount\": 3000,\r\n      \"line_item/tag\": \"\",\r\n    },...\r\n]"
                },
                //new()
                //{
                //    InlineData = new()
                //    {
                //        MimeType = "image/png",
                //        Data = forbiddenCity
                //    }
                //},
                //new()
                //{
                //    Text = "city: Beijing, Landmark: Forbidden City"
                //},
                //new()
                //{
                //    InlineData = new()
                //    {
                //        MimeType = "image/png",
                //        Data = christRedeemer
                //    }
                //}
            });

            var generateContentRequest = new GenerateContentRequest
            {
                Model = $"projects/{projectId}/locations/{ConstantConfig.LOCATION}/publishers/{publisher}/models/{model}"
            };
            generateContentRequest.Contents.Add(content);

            // Make the request, returning a streaming response
            using PredictionServiceClient.StreamGenerateContentStream response = predictionServiceClient.StreamGenerateContent(generateContentRequest);

            StringBuilder fullText = new();

            // Read streaming responses from server until complete
            AsyncResponseStream<GenerateContentResponse> responseStream = response.GetResponseStream();
            await foreach (GenerateContentResponse responseItem in responseStream)
            {
                fullText.Append(responseItem.Candidates[0].Content.Parts[0].Text);
            }
            // log the result
            Console.WriteLine("log the result: ", fullText.ToString());
            return fullText.ToString();
        }

        private static async Task<ByteString> ReadImageFileAsync(string url)
        {
            using HttpClient client = new();
            using var response = await client.GetAsync(url);
            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            return ByteString.CopyFrom(imageBytes);
        }
    }
}
