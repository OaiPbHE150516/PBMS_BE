﻿using Google.Apis.Auth.OAuth2;
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

        public static string UploadFile(IFormFile file, string prefix, string suffix)
        {
            var storage = StorageClient.Create();
            var fileUpload = file.OpenReadStream();
            // file name = file name + date time now timestamp
            var fileName = prefix + "_" + file.FileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + suffix;
            storage.UploadObject(ConstantConfig.BUCKET_NAME, fileName, null, fileUpload);
            return fileName;
        }
    }
}
