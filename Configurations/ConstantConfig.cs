namespace pbms_be.Configurations
{
    public class ConstantConfig
    {
        public const int ADMIN_ROLE_ID = 1;
        public const int USER_ROLE_ID = 2;

        public const string ADMIN_ROLE_NAME = "admin";
        public const string USER_ROLE_NAME = "user";

        // Vision status
        public const string VISION_STATUS_HIDDEN = "hidden";
        public const string VISION_STATUS_VISIBLE = "visible";
        public const string VISION_STATUS_DELETED = "deleted";

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
