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

        // NUMBER_LAST_DAYS
        public const int NUMBER_LAST_DAYS = 7;

        // Bucket config
        public const string BUCKET_NAME = "pbms-user";

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

        public const string DEFAULT_UTC_DATETIME_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";
        // default time format
        public const string DEFAULT_TIME_FORMAT = "HH:mm";
        // default date format
        public const string DEFAULT_DATE_FORMAT = "dd/MM/yyyy";

        public const string DEFAULT_DATE_FORMAT_DASH = "dd-MM-yyyy";
        // DEFAULT_DATE_FORMAT_SLASH
        public const string DEFAULT_DATE_FORMAT_SLASH = "dd/MM/yy";

        // DEFAULT_DATE_FORMAT_SHORT
        public const string DEFAULT_DATE_FORMAT_SHORT = "dd thg MM";

        public const string DEFAULT_DAY_THG_MONTH = " thg ";

        // default category name en
        public const string DEFAULT_CATEGORY_NAME_EN_INCOME = "Income";
        public const string DEFAULT_CATEGORY_NAME_VN_INCOME = "Thu nhập";
        public const string DEFAULT_CATEGORY_NAME_EN_EXPENSE = "Expense";
        public const string DEFAULT_CATEGORY_NAME_VN_EXPENSE = "Chi tiêu";
        public const string DEFAULT_CATEGORY_NAME_EN_OTHER = "Other";
        public const string DEFAULT_CATEGORY_NAME_VN_OTHER = "Khác";

        public const int DEFAULT_CATEGORY_TYPE_ID_INCOME = 1;
        public const int DEFAULT_CATEGORY_TYPE_ID_EXPENSE = 2;
        public const int DEFAULT_CATEGORY_TYPE_ID_OTHER = 3;

        // default Sort & Filter
        // public const string ascending sort
        public const string ASCENDING_SORT = "asc";
        // public const string descending sort
        public const string DESCENDING_SORT = "desc";
        // public const string sort type

        // default VND thousand separator dong format
        public const int DEFAULT_VND_THOUSAND_SEPARATOR = 1000;
        public const int FILE_SIZE_LIMIT = 20971520;

    }

    // Message to response to client
    public class Message
    {
        public const string ACCOUNT_ID_REQUIRED = " AccountID is required";
        public const string INVOICE_ID_REQUIRED = " InvoiceID is required";
        public const string FILE_IS_NULL = " File is null or not of type pdf";
        public const string FILE_IS_NOT_PDF_JPG_PNG = " File is null or not of type pdf, jpg or png";

        // DATE_TIME_REQUIRED
        public const string DATE_TIME_REQUIRED = " Date time is required";

        public const string NUMBER_REQUIRED = " Number is required";

        // Page
        public const string PAGE_NOT_FOUND = " Page not found";
        public const string PAGE_NUMBER_REQUIRED = " Page number is required";
        public const string PAGE_SIZE_REQUIRED = " Page size is required";

        // Sort & Filter
        public const string SORT_TYPE_REQUIRED = " Sort type is required";

        // Transaction
        public const string TRANSACTION_NOT_FOUND = " Transaction not found";
        public const string TRANSACTION_ID_REQUIRED = " TransactionID is required";
        public const string TRANSACTION_CREATE_FAILED = " Transaction create failed";
        public const string TRANSACTION_EXISTED = " Transaction existed";
        public const string DAY_REQUIRED = " Day is required";
        public const string MONTH_REQUIRED = " Month is required";
        public const string YEAR_REQUIRED = " Year is required";
        public const string FROM_DATE_REQUIRED = " From date is required";
        public const string TO_DATE_REQUIRED = " To date is required";
        public const string TRANSACTION_IS_NULL = " Transaction is null";
        public const string WALLET_NOT_BELONG_ACCOUNT = " Wallet not belong to account";
        public const string CATEGORY_NOT_BELONG_ACCOUNT = " Category not belong to account";


        // Budget
        public const string BUDGET_ALREADY_EXIST = " Budget already exist";
        public const string BUDGET_ID_REQUIRED = " BudgetID is required";
        public const string BUDGET_NOT_FOUND = " Budget not found";
        public const string BUDGET_TYPE_NOT_FOUND = " Budget type not found";

        // Balance
        public const string BALANCE_HISTORY_LOG_NOT_FOUND = "No balance history log found for this account";
        public const string BALANCE_HISTORY_LOG_INVALID = "Balance history log is invalid";

        // Category
        public const string CATEGORY_ALREADY_EXIST = " Category already exist";
        public const string CATEGORY_ID_REQUIRED = " CategoryID is required";
        public const string CATEGORY_NOT_FOUND = " Category not found";
        public const string CATEGORY_PARENT_NOT_FOUND = " Category parent not found";

        // Category type
        public const string CATEGORY_TYPE_NOT_FOUND = " Category type not found";

        // email address is required
        public const string EMAIL_ADDRESS_REQUIRED = " Email address is required";

        // Collab fund
        public const string COLLAB_FUND_ALREADY_EXIST = " Collab fund already exist";
        public const string COLLAB_FUND_ID_REQUIRED = " CollabFundID is required";
        public const string COLLAB_FUND_NOT_EXIST = " Collab fund not exist";
        public const string COLLAB_FUND_DUPLICATE = " Collab fund duplicate";
        public const string COLLAB_FUND_ACTIVITY_DUPLICATE = " Collab fund activity duplicate";
        public const string ACCOUNT_IS_NOT_FUNDHOLDER = " Account is not fundholder";
        public const string ACCOUNT_NOT_FOUND = " Account not found";
        public const string ACCOUNT_IS_NOT_MEMBER = " Account is not member";
        public const string ACCOUNT_ALREADY_IS_MEMBER = " Account is member";
        public const string ACCOUNT_ALREADY_IS_FUNDHOLDER = " Account is fundholder";
        public const string ACCOUNT_IS_NOT_IN_COLLAB_FUND = " Account is not in collab fund";
        public const string ACCOUNT_ALREADY_INVITED = " Account is already invited";
        public const string ACCOUNT_ALREADY_ACCEPTED = " Account is already accepted";
        public const string ACCOUNT_ALREADY_REJECTED =  "Account is already rejected";
        public const string ACCOUNT_WAS_NOT_INVITED = " Account was not invited";
        public const string ACCOUNT_FROM_NOT_FOUND = " Account from not found";
        public const string ACCOUNT_TO_NOT_FOUND = " Account to not found";
        public const string COLLAB_FUND_ACTIVITY_NOT_FOUND = " Collab fund activity not found";

        // cfdm detail not found
        public const string CFDM_DETAIL_NOT_FOUND = " Collab Fund dividing money detail not found";

        // just now
        public const string VN_JUST_NOW = "vừa xong";
        public const string VN_MINUTES_AGO = " phút trước";
        public const string VN_HOURS_AGO = " giờ trước";
        public const string VN_DAYS_AGO = " ngày trước";
        public const string VN_MONTHS_AGO = " tháng trước";


        // Mapper
        public const string MAPPER_IS_NULL = " Mapper is null";

        // Activity 


        // Wallet
        public const string WALLET_ID_REQUIRED = " WalletID is required";
        public const string WALLET_NOT_FOUND = " Wallet not found";

        // File
        public const string FILE_NOT_FOUND = " File not found";
        public const string FILE_NAME_REQUIRED = " File name is required";
        public const string FILE_IS_NULL_ = " File is null";
        public const string FILE_IS_TOO_LARGE = " File is too large";
        public const string FILE_IS_NOT_JPG_PNG = " File is not of type jpg or png";

        // Datetime
        public const string FROM_DATE_GREATER_THAN_TO_DATE = " From date greater than to date";
        // VN_MONDAY
        public const string VN_MONDAY = "Thứ Hai";
        public const string VN_MONDAY_SHORT_1 = "T2";
        public const string VN_MONDAY_SHORT_2 = "t2";
        public const string VN_MONDAY_SHORT_3 = "Th 2";
        public const string VN_MONDAY_SHORT_4 = "Th Hai";
        public const string VN_MONDAY_SHORT_5 = "Thứ 2";

        // VN_TUESDAY
        public const string VN_TUESDAY = "Thứ Ba";
        public const string VN_TUESDAY_SHORT_1 = "T3";
        public const string VN_TUESDAY_SHORT_2 = "t3";
        public const string VN_TUESDAY_SHORT_3 = "Th 3";
        public const string VN_TUESDAY_SHORT_4 = "Th Ba";
        public const string VN_TUESDAY_SHORT_5 = "Thứ 3";

        // VN_WEDNESDAY
        public const string VN_WEDNESDAY = "Thứ Tư";
        public const string VN_WEDNESDAY_SHORT_1 = "T4";
        public const string VN_WEDNESDAY_SHORT_2 = "t4";
        public const string VN_WEDNESDAY_SHORT_3 = "Th 4";
        public const string VN_WEDNESDAY_SHORT_4 = "Th Tư";
        public const string VN_WEDNESDAY_SHORT_5 = "Thứ 4";

        // VN_THURSDAY
        public const string VN_THURSDAY = "Thứ Năm";
        public const string VN_THURSDAY_SHORT_1 = "T5";
        public const string VN_THURSDAY_SHORT_2 = "t5";
        public const string VN_THURSDAY_SHORT_3 = "Th 5";
        public const string VN_THURSDAY_SHORT_4 = "Th Năm";
        public const string VN_THURSDAY_SHORT_5 = "Thứ 5";

        // VN_FRIDAY
        public const string VN_FRIDAY = "Thứ Sáu";
        public const string VN_FRIDAY_SHORT_1 = "T6";
        public const string VN_FRIDAY_SHORT_2 = "t6";
        public const string VN_FRIDAY_SHORT_3 = "Th 6";
        public const string VN_FRIDAY_SHORT_4 = "Th Sáu";
        public const string VN_FRIDAY_SHORT_5 = "Thứ 6";

        // VN_SATURDAY
        public const string VN_SATURDAY = "Thứ Bảy";
        public const string VN_SATURDAY_SHORT_1 = "T7";
        public const string VN_SATURDAY_SHORT_2 = "t7";
        public const string VN_SATURDAY_SHORT_3 = "Th 7";
        public const string VN_SATURDAY_SHORT_4 = "Th Bảy";
        public const string VN_SATURDAY_SHORT_5 = "Thứ 7";

        // VN_SUNDAY
        public const string VN_SUNDAY = "Chủ Nhật";
        public const string VN_SUNDAY_SHORT_1 = "CN";
        public const string VN_SUNDAY_SHORT_2 = "cn";
        public const string VN_SUNDAY_SHORT_3 = "CN";
        public const string VN_SUNDAY_SHORT_4 = "Chủ Nhật";
        public const string VN_SUNDAY_SHORT_5 = "Chủ Nhật";

        // VN_DAY
        public const string VN_DAY = "ngày";
        public const string VN_DAY_SHORT = "ng";

        // VN_MONTH
        public const string VN_MONTH = "tháng";
        public const string VN_MONTH_SHORT = "thg";

        // VN_YEAR
        public const string VN_YEAR = "năm";

        // VN_JANUARY
        public const string VN_JANUARY = "Tháng Một";
        public const string VN_JANUARY_SHORT_1 = "Thg 1";

        // VN_FEBRUARY
        public const string VN_FEBRUARY = "Tháng Hai";
        public const string VN_FEBRUARY_SHORT_1 = "Thg 2";

        // VN_MARCH
        public const string VN_MARCH = "Tháng Ba";
        public const string VN_MARCH_SHORT_1 = "Thg 3";

        // VN_APRIL
        public const string VN_APRIL = "Tháng Tư";
        public const string VN_APRIL_SHORT_1 = "Thg 4";

        // VN_MAY
        public const string VN_MAY = "Tháng Năm";
        public const string VN_MAY_SHORT_1 = "Thg 5";

        // VN_JUNE
        public const string VN_JUNE = "Tháng Sáu";
        public const string VN_JUNE_SHORT_1 = "Thg 6";

        // VN_JULY
        public const string VN_JULY = "Tháng Bảy";
        public const string VN_JULY_SHORT_1 = "Thg 7";

        // VN_AUGUST
        public const string VN_AUGUST = "Tháng Tám";
        public const string VN_AUGUST_SHORT_1 = "Thg 8";

        // VN_SEPTEMBER
        public const string VN_SEPTEMBER = "Tháng Chín";
        public const string VN_SEPTEMBER_SHORT_1 = "Thg 9";

        // VN_OCTOBER
        public const string VN_OCTOBER = "Tháng Mười";
        public const string VN_OCTOBER_SHORT_1 = "Thg 10";

        // VN_NOVEMBER
        public const string VN_NOVEMBER = "Tháng Mười Một";
        public const string VN_NOVEMBER_SHORT_1 = "Thg 11";

        // VN_DECEMBER
        public const string VN_DECEMBER = "Tháng Mười Hai";
        public const string VN_DECEMBER_SHORT_1 = "Thg 12";



    }

    public class CloudStorageConfig
    {
        public const string PUBLIC_URL = "https://storage.googleapis.com/";
        public const string PBMS_BUCKET_NAME = "pbms-user";
        public const string INVOICE_FOLDER = "invoice";
        public const string COLLAB_FUND_FOLDER = "collabfund";
        public const string FILE_FOLDER = "file";
        // pbms-user/invoice default = public_url + pbms-user/invoice
        public const string INVOICE_DEFAULT_FOLDER = PUBLIC_URL + "/" + PBMS_BUCKET_NAME + "/" + INVOICE_FOLDER;
        // pbms-user/file default = public_url + pbms-user/file
        public const string FILE_DEFAULT_FOLDER = PUBLIC_URL + "/" + PBMS_BUCKET_NAME + "/" + FILE_FOLDER;
        //         public static string UploadFileCustom(IFormFile file, string bucketname, string folder, string prefix, string filename, string suffix, bool isWithDatetime)

        public const int DEFAULT_FILE_NAME_LENGTH = 15;
    }

    public class  CurrencyConst
    {
        public const string dVND = "đ";
        public const int DEFAULT_CURRENCY_ID_VND = 2;
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
