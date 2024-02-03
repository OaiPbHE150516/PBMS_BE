using Google.Cloud.DocumentAI.V1;
using Google.Protobuf;
using pbms_be.Configurations;
namespace pbms_be.ThirdParty
{
    public class DocumentAiApi
    {
        //static string projectId = "lexical-aileron-410114";
        //static string location = "us";
        //static string processorId = "6036d75c63c4564";
        ////static string filePath = "C:\\Users\\Phamb\\Downloads\\mau-bill-an-uong-9_1597330928.pdf";
        //static string filePath = "C:\\Users\\Phamb\\Downloads\\Invoice.pdf";
        //static string mineType = "application/pdf";

        //public static Document ProcessDocument(IFormFile file)
        //{
        //    // Create client
        //    var client = new DocumentProcessorServiceClientBuilder
        //    {
        //        Endpoint = $"{location}-documentai.googleapis.com"
        //    }.Build();

        //    // Read in local file
        //    using var fileStream = File.OpenRead(filePath);
        //    var rawDocument = new RawDocument
        //    {
        //        Content = ByteString.FromStream(fileStream),
        //        MimeType = mineType
        //    };

        //    // Initialize request argument(s)
        //    var request = new ProcessRequest
        //    {
        //        Name = ProcessorName.FromProjectLocationProcessor(projectId, location, processorId).ToString(),
        //        RawDocument = rawDocument
        //    };

        //    // Make the request
        //    var response = client.ProcessDocument(request);

        //    var document = response.Document;
        //    Console.WriteLine(document.Text);
        //    return document;
        //}

        public static Document ProcessDocument(IFormFile file)
        {
            // create client
            var client = new DocumentProcessorServiceClientBuilder
            {
                Endpoint = $"{ConstantConfig.LOCATION}-documentai.googleapis.com"
            }.Build();

            // read file
            var content = file.OpenReadStream();
            var rawDocument = new RawDocument
            {
                Content = ByteString.FromStream(content),
                MimeType = ConstantConfig.MINE_TYPE_JPEG
            };

            // Initialize request argument(s)
            var request = new ProcessRequest
            {
                Name = ProcessorName.FromProjectLocationProcessor(ConstantConfig.PROJECT_ID, ConstantConfig.LOCATION, ConstantConfig.PROCESSOR_ID).ToString(),
                RawDocument = rawDocument
            };

            // Make the request
            var response = client.ProcessDocument(request);
            var document = response.Document;
            return document;
        }
    }
}
