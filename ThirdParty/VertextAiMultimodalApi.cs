using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;
using Google.Protobuf;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace pbms_be.ThirdParty
{
    public class VertextAiMultimodalApi
    {
        public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "us-central1",
        string publisher = "google",
        string model = "gemini-1.0-pro-vision"
    )
        {
            // Create client
            var predictionServiceClient = new PredictionServiceClientBuilder
            {
                Endpoint = $"{location}-aiplatform.googleapis.com"
            }.Build();

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
                Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}"
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
