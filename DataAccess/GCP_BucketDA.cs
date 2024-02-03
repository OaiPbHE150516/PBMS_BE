﻿using Google.Cloud.Storage.V1;
using pbms_be.Configurations;

namespace pbms_be.DataAccess
{
    public class GCP_BucketDA
    {
        public static void UploadFile(IFormFile file)
        {
            var storage = StorageClient.Create();
            var fileUpload = file.OpenReadStream();
            // file name = file name + date time now timestamp
            var fileName = file.FileName + DateTime.Now.ToString("yyyyMMddHHmmss");
            storage.UploadObject(ConstantConfig.BUCKET_NAME, fileName, null, fileUpload);
        }
    }
}
