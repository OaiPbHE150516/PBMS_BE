using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using pbms_be.Configurations;

namespace pbms_be.DataAccess
{
    public class GCP_BucketDA
    {
        public static string UploadFile(IFormFile file)
        {
            // https://github.com/dotnet/runtime/issues/94794 to fix permission issue
            //var linkcredential = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            //var credential = GoogleCredential.FromFile(linkcredential);
            var storage = StorageClient.Create();
            var fileUpload = file.OpenReadStream();
            // file name = file name + date time now timestamp
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;
            storage.UploadObject(ConstantConfig.BUCKET_NAME, fileName, null, fileUpload);
            return fileName;
        }

        public static string UploadFile(IFormFile file, string prefix, string folder)
        {
            var pre = prefix ?? "file";
            var storage = StorageClient.Create();
            var fileUpload = file.OpenReadStream();
            var contentType = file.ContentType;
            // file name = file name + date time now timestamp
            var fileName = folder + "/" + pre + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;
            storage.UploadObject(ConstantConfig.BUCKET_NAME, fileName, contentType, fileUpload);
            return fileName;
        }

        public static string UploadFileCustom(IFormFile file, string bucketname, string folder, string prefix, string filename, string suffix, bool isWithDatetime)
        {
            try
            {
                // throw exception if file is null or too large > 20MB
                if (file == null) throw new Exception(Message.FILE_IS_NULL_);
                if (file.Length > ConstantConfig.FILE_SIZE_LIMIT) throw new Exception(Message.FILE_IS_TOO_LARGE);
                // nếu prefix = null thì prefix = file, nếu không thì prefix = prefix
                prefix = prefix ?? "file";
                // nếu suffix = null thì suffix = file, nếu không thì suffix = suffix
                suffix = suffix ?? "file";
                // nếu filename = null thì filename = file + datetime to string, nếu không thì filename = filename
                filename = filename ?? "file" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var storage = StorageClient.Create();
                var fileUpload = file.OpenReadStream();
                var contentType = file.ContentType;
                // get file name type from file.FileName like .jpg, .png, .pdf
                var fileExtension = Path.GetExtension(file.FileName);
                // file name = file name + date time now timestamp
                var fileName = folder + "/" + prefix + "_" + filename + "_" + suffix + fileExtension;
                if (isWithDatetime)
                {
                    fileName = folder + "/" + prefix + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + filename + "_" + suffix + fileExtension;
                }
                storage.UploadObject(bucketname, fileName, contentType, fileUpload);

                var url = CloudStorageConfig.PUBLIC_URL + "/" + bucketname + "/" + fileName;
                return url;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //public static string UploadFileInternal(IFormFile file, )

        //public static string DownloadFile(string fileName)
        //{
        //    //var storage = StorageClient.Create();
        //    //var file = storage.DownloadObject(ConstantConfig.BUCKET_NAME, fileName);
        //    //return file;
        //    return null;
        //}
    }
}
