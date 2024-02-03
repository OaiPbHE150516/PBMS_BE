namespace pbms_be.Data.Material
{
    public class FileBundle
    {
        #region constructor
        public FileBundle(HttpRequest request)
        {
            _request = request;
        }
        private HttpRequest _request { get; set; }
        #endregion

        public int ReturnCode { get; set; }
        public string Message { get; set; } = String.Empty;

    }
}
