namespace pbms_be.Configurations
{
    public class ConstantConfig
    {
        public const int ADMIN_ROLE_ID = 1;
        public const int USER_ROLE_ID = 2;

        public const string ADMIN_ROLE_NAME = "admin";
        public const string USER_ROLE_NAME = "user";

        // Vision status
        //public const string VISION_STATUS_HIDDEN = "hidden";
        //public const string VISION_STATUS_VISIBLE = "visible";
        //public const string VISION_STATUS_DELETED = "deleted";

        // Account information
        public const string TOKEN_NAME = "name";
        public const string TOKEN_CLIENT_UNIQUEID = "sub";
        public const string TOKEN_CLIENT_ID = "aud";
        public const string TOKEN_CLIENT_EMAIL = "email";
        public const string TOKEN_CLIENT_PICTURE = "picture";

        // processor config
        // projectid, location, processorid, mineType_PDF, mineType_JPEG
        public const string PROJECT_ID = "lexical-aileron-410114";
        public const string LOCATION = "us";
        public const string PROCESSOR_ID = "6036d75c63c4564";
        public const string MINE_TYPE_PDF = "application/pdf";
        public const string MINE_TYPE_JPEG = "image/jpeg";
        public const string MINE_TYPE_PNG = "image/png";
        public const string MINE_TYPE_JPG = "image/jpg";

        // Bucket config
        public const string BUCKET_NAME = "user_invoice";

        // Value
        public const long NEGATIVE_VALUE = -1;
        public const int DEFAULT_ACTIVE_STATE_VALUE = 1;
        // Dùng trong trường hợp không có transaction id
        public const int DEFAULT_NULL_TRANSACTION_ID = 1;
        // default primary key
        public const int DEFAULT_PRIMARY_KEY = 1;
        // default value for zero
        public const int DEFAULT_ZERO_VALUE  = 0;

        // VN_TIMEZONE_UTC
        public const int VN_TIMEZONE_UTC = 7;

        // min minutes ago to know it just now
        public const int MIN_MINUTES_AGO = 1;
        // min hours ago to know it minutes ago
        public const int MIN_HOURS_AGO = 60;
        // min days ago to know it hours ago
        public const int MIN_DAYS_AGO = 24;
        // min months ago to know it days ago
        public const int MIN_MONTHS_AGO = 30;

        public const string DEFAULT_DATETIME_FORMAT = "HH:mm, dd/MM/yyyy";
        // default time format
        public const string DEFAULT_TIME_FORMAT = "HH:mm";
        // default date format
        public const string DEFAULT_DATE_FORMAT = "dd/MM/yyyy";

        // default VND thousand separator dong format
        public const int DEFAULT_VND_THOUSAND_SEPARATOR = 1000;

    }

    // Message to response to client
    public class Message
    {
        public const string ACCOUNT_ID_REQUIRED = "AccountID is required";
        public const string INVOICE_ID_REQUIRED = "InvoiceID is required";
        public const string FILE_IS_NULL = "File is null or not of type pdf";
        public const string FILE_IS_NOT_PDF_JPG_PNG = "File is null or not of type pdf, jpg or png";
        // email address is required
        public const string EMAIL_ADDRESS_REQUIRED = "Email address is required";

        // Collab fund
        public const string COLLAB_FUND_ALREADY_EXIST = "Collab fund already exist";
        public const string COLLAB_FUND_ID_REQUIRED = "CollabFundID is required";
        public const string COLLAB_FUND_NOT_EXIST = "Collab fund not exist";
        public const string COLLAB_FUND_DUPLICATE = "Collab fund duplicate";
        public const string COLLAB_FUND_ACTIVITY_DUPLICATE = "Collab fund activity duplicate";
        public const string ACCOUNT_IS_NOT_FUNDHOLDER = "Account is not fundholder";
        public const string ACCOUNT_NOT_FOUND = "Account not found";
        public const string ACCOUNT_IS_NOT_MEMBER = "Account is not member";
        public const string ACCOUNT_ALREADY_IS_MEMBER = "Account is member";
        public const string ACCOUNT_ALREADY_IS_FUNDHOLDER = "Account is fundholder";
        public const string ACCOUNT_IS_NOT_IN_COLLAB_FUND = "Account is not in collab fund";
        public const string ACCOUNT_ALREADY_INVITED = "Account is already invited";
        public const string ACCOUNT_ALREADY_ACCEPTED = "Account is already accepted";
        public const string ACCOUNT_ALREADY_REJECTED = "Account is already rejected";
        public const string ACCOUNT_WAS_NOT_INVITED = "Account was not invited";

        // just now
        public const string VN_JUST_NOW = "vừa xong";
        public const string VN_MINUTES_AGO = " phút trước";
        public const string VN_HOURS_AGO = " giờ trước";
        public const string VN_DAYS_AGO = " ngày trước";
        public const string VN_MONTHS_AGO = " tháng trước";


        // Mapper
        public const string MAPPER_IS_NULL = "Mapper is null";

        // Activity 
        public const string COLLAB_FUND_ACTIVITY_NOT_FOUND = "Collab fund activity not found";
    }

    public class ActiveStateConst
    {
        public const int ACTIVE = 1;
        public const string ACTIVE_NAME = "active";
        public const int INACTIVE = 2;
        public const string INACTIVE_NAME = "inactive";
        public const int PENDING = 3;
        public const string PENDING_NAME = "pending";
        public const int SUSPENDED = 4;
        public const string SUSPENDED_NAME = "suspended";
        public const int DELETED = 5;
        public const string DELETED_NAME = "deleted";
    }

    public class InvoiceConfig
    {
        // Invoice config
        // invoice_date, invoice_id, currency
        public const string INVOICE_DATE = "invoice_date";
        public const string INVOICE_ID = "invoice_id";
        public const string CURRENCY = "currency";
        // invoice-discount, invoice-note, 
        public const string INVOICE_DISCOUNT = "discount";
        public const string INVOICE_NOTE = "note";
        // net_amount, payment_terms, ship_to_address, ship_to_name, 
        public const string NET_AMOUNT = "net_amount";
        public const string PAYMENT_TERMS = "payment_terms";
        public const string SHIP_TO_ADDRESS = "ship_to_address";
        public const string SHIP_TO_NAME = "ship_to_name";
        // supplier_address, supplier_email, supplier_name, supplier_phone, supplier_website
        public const string SUPPLIER_ADDRESS = "supplier_address";
        public const string SUPPLIER_EMAIL = "supplier_email";
        public const string SUPPLIER_NAME = "supplier_name";
        public const string SUPPLIER_PHONE = "supplier_phone";
        public const string SUPPLIER_WEBSITE = "supplier_website";
        // total_amount, total_tax_amount
        public const string TOTAL_AMOUNT = "total_amount";
        public const string TOTAL_TAX_AMOUNT = "total_tax_amount";
        // line_item, line_item_description, line_item_quantity, line_item_unit, line_item_unit_price, line_item_amount
        public const string LINE_ITEM = "line_item";
        public const string LINE_ITEM_DESCRIPTION = "line_item/description";
        public const string LINE_ITEM_QUANTITY = "line_item/quantity";
        public const string LINE_ITEM_UNIT = "line_item/unit";
        public const string LINE_ITEM_UNIT_PRICE = "line_item/unit_price";
        public const string LINE_ITEM_AMOUNT = "line_item/amount";
    }
}
