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
        public static async Task<string> GenerateContent()
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

            //    // Prompt
            //    string prompt = "What's in this photo";
            //    string imageUri = "gs://generativeai-downloads/images/scones.jpg";

            //    // Initialize request argument(s)
            //    var content = new Content
            //    {
            //        Role = "USER"
            //    };
            //    content.Parts.AddRange(new List<Part>()
            //{
            //    new() {
            //        Text = prompt
            //    },
            //    new() {
            //        FileData = new() {
            //            MimeType = "image/png",
            //            FileUri = imageUri
            //        }
            //    }
            //});

            //    var generateContentRequest = new GenerateContentRequest
            //    {
            //        Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
            //        GenerationConfig = new GenerationConfig
            //        {
            //            Temperature = 0.4f,
            //            TopP = 1,
            //            TopK = 32,
            //            MaxOutputTokens = 2048
            //        }
            //    };
            //    generateContentRequest.Contents.Add(content);

            //    // Make the request, returning a streaming response
            //    using PredictionServiceClient.StreamGenerateContentStream response = predictionServiceClient.StreamGenerateContent(generateContentRequest);

            //    StringBuilder fullText = new();

            //    // Read streaming responses from server until complete
            //    AsyncResponseStream<GenerateContentResponse> responseStream = response.GetResponseStream();
            //    await foreach (GenerateContentResponse responseItem in responseStream)
            //    {
            //        fullText.Append(responseItem.Candidates[0].Content.Parts[0].Text);
            //    }

            //    return fullText.ToString();
            //}

            // Images
            ByteString colosseum = await ReadImageFileAsync(
                "https://storage.googleapis.com/cloud-samples-data/vertex-ai/llm/prompts/landmark1.png");

            ByteString forbiddenCity = await ReadImageFileAsync(
                "https://storage.googleapis.com/cloud-samples-data/vertex-ai/llm/prompts/landmark2.png");

            ByteString christRedeemer = await ReadImageFileAsync(
                "https://storage.googleapis.com/cloud-samples-data/vertex-ai/llm/prompts/landmark3.png");

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
                    MimeType = "image/png",
                    Data = colosseum

                }
            },
            new()
            {
                Text = "city: Rome, Landmark: the Colosseum"
            },
            new()
            {
                InlineData = new()
                {
                    MimeType = "image/png",
                    Data = forbiddenCity
                }
            },
            new()
            {
                Text = "city: Beijing, Landmark: Forbidden City"
            },
            new()
            {
                InlineData = new()
                {
                    MimeType = "image/png",
                    Data = christRedeemer
                }
            }
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
