namespace pbms_be.Configurations
{
    public class ConstantConfig
    {
        public static int ADMIN_ROLE_ID = 1;
        public static int USER_ROLE_ID = 2;

        public static string ADMIN_ROLE_NAME = "admin";
        public static string USER_ROLE_NAME = "user";

        // Vision status
        public static string VISION_STATUS_HIDDEN = "hidden";
        public static string VISION_STATUS_VISIBLE = "visible";
        public static string VISION_STATUS_DELETED = "deleted";

        // Account information
        public static string TOKEN_NAME = "name";
        public static string TOKEN_CLIENT_UNIQUEID = "sub";
        public static string TOKEN_CLIENT_ID = "aud";
        public static string TOKEN_CLIENT_EMAIL = "email";
        public static string TOKEN_CLIENT_PICTURE = "picture";

        // processor config
        // projectid, location, processorid, mineType_PDF, mineType_JPEG
        public static string PROJECT_ID = "lexical-aileron-410114";
        public static string LOCATION = "us";
        public static string PROCESSOR_ID = "6036d75c63c4564";
        public static string MINE_TYPE_PDF = "application/pdf";
        public static string MINE_TYPE_JPEG = "image/jpeg";

    }
}
