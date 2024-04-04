using Google.Cloud.DocumentAI.V1;
using Google.Protobuf;
using Google.Protobuf.Collections;
using pbms_be.Configurations;
using pbms_be.Data.Custom;
using pbms_be.Data.Invo;

namespace pbms_be.ThirdParty
{
    public class DocumentAiApi
    {

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
                MimeType = file.ContentType
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

        // get text from document
        public static Invoice GetInvoiceFromDocument(Document document)
        {
            try
            {
                var entities = document.Entities;
                Invoice invoice = new Invoice();
                // loop through entities
                var countProduct = 0;
                foreach (var entity in entities)
                {
                    if (entity.Properties.Count > 0)
                    {
                        countProduct++;
                        var productInInvoice = GetProductInInvoice(entity.Properties);
                        productInInvoice.ProductID = countProduct;
                        invoice.ProductInInvoices.Add(productInInvoice);
                        continue;
                    }
                    var type = entity.Type;
                    switch (entity.Type)
                    {
                        case InvoiceConfig.CURRENCY:
                            //invoice.CurrencyID = entity.MentionText;
                            invoice.CurrencyID = 2;
                            break;
                        case InvoiceConfig.INVOICE_ID:
                            // convert to int
                            invoice.IDOfInvoice = entity.MentionText;
                            break;
                        case InvoiceConfig.INVOICE_DATE:
                            //var invoicedate = ConvertStringToDate(entity.MentionText);
                            // use TryConvertStringToDate to check if the date is valid
                            if (TryConvertStringToDate(entity.MentionText, out DateTime resultInvoiceDate))
                            {
                                invoice.InvoiceDate = resultInvoiceDate;
                            }
                            break;
                        case InvoiceConfig.INVOICE_DISCOUNT:
                            //invoice.Discount = (long)Convert.ToDouble(entity.MentionText);
                            if (TryConvertStringToLong(entity.MentionText, out long resultDiscount))
                            {
                                invoice.Discount = resultDiscount;
                            }
                            else
                            {
                                invoice.Discount = -1;
                            }
                            break;
                        case InvoiceConfig.INVOICE_NOTE:
                            invoice.Note = entity.MentionText;
                            break;
                        case InvoiceConfig.NET_AMOUNT:
                            //invoice.NetAmount = (long)Convert.ToDouble(entity.MentionText);
                            if (TryConvertStringToLong(entity.MentionText, out long resultNetAmount))
                            {
                                invoice.NetAmount = resultNetAmount;
                            }
                            else
                            {
                                invoice.NetAmount = ConstantConfig.NEGATIVE_VALUE;
                            }
                            break;
                        case InvoiceConfig.PAYMENT_TERMS:
                            invoice.PaymentTerms = entity.MentionText;
                            break;
                        case InvoiceConfig.SHIP_TO_ADDRESS:
                            invoice.ReceiverAddress = entity.MentionText;
                            break;
                        case InvoiceConfig.SHIP_TO_NAME:
                            invoice.ReceiverName = entity.MentionText;
                            break;
                        case InvoiceConfig.SUPPLIER_ADDRESS:
                            invoice.SupplierAddress = entity.MentionText;
                            break;
                        case InvoiceConfig.SUPPLIER_EMAIL:
                            invoice.SupplierEmail = entity.MentionText;
                            break;
                        case InvoiceConfig.SUPPLIER_NAME:
                            invoice.SupplierName = entity.MentionText;
                            break;
                        case InvoiceConfig.SUPPLIER_PHONE:
                            invoice.SupplierPhone = entity.MentionText;
                            break;
                        //case ConstantConfig.SUPPLIER_WEBSITE:
                        //    invoice.SupplierWebsite = entity.MentionText;
                        //    break;
                        case InvoiceConfig.TOTAL_AMOUNT:
                            if (TryConvertStringToLong(entity.MentionText, out long resultTotalAmount))
                            {
                                invoice.TotalAmount = resultTotalAmount;
                            }
                            else
                            {
                                invoice.TotalAmount = ConstantConfig.NEGATIVE_VALUE;
                            }
                            break;
                        case InvoiceConfig.TOTAL_TAX_AMOUNT:
                            if (TryConvertStringToLong(entity.MentionText, out long resultTaxAmount))
                            {
                                invoice.TaxAmount = resultTaxAmount;
                            }
                            else
                            {
                                invoice.TaxAmount = ConstantConfig.NEGATIVE_VALUE;
                            }
                            break;
                    }
                    // end of switch case
                }
                // end of loop
                invoice.InvoiceImageURL = "url";
                invoice.InvoiceRawDatalog = entities.ToString();
                invoice.ActiveStateID = ConstantConfig.DEFAULT_ACTIVE_STATE_VALUE;
                return invoice;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static ProductInInvoice GetProductInInvoice(RepeatedField<Document.Types.Entity> properties)
        {
            ProductInInvoice productInInvoice = new ProductInInvoice();
            foreach (var property in properties)
            {
                switch (property.Type)
                {
                    case InvoiceConfig.LINE_ITEM_DESCRIPTION:
                        productInInvoice.ProductName = property.MentionText;
                        break;
                    case InvoiceConfig.LINE_ITEM_QUANTITY:
                        //productInInvoice.Quanity = Convert.ToInt32(property.MentionText);
                        Console.WriteLine(InvoiceConfig.LINE_ITEM_QUANTITY + ": " + property.MentionText);
                        break;
                    case InvoiceConfig.LINE_ITEM_UNIT_PRICE:
                        if (TryConvertStringToLong(property.MentionText, out long resultUnitPrice))
                        {
                            productInInvoice.UnitPrice = resultUnitPrice;
                        }
                        else
                        {
                            productInInvoice.UnitPrice = ConstantConfig.NEGATIVE_VALUE;
                        }
                        break;
                    case InvoiceConfig.LINE_ITEM_AMOUNT:
                        if (TryConvertStringToLong(property.MentionText, out long resultTotalAmount))
                        {
                            productInInvoice.TotalAmount = resultTotalAmount;
                        }
                        else
                        {
                            productInInvoice.TotalAmount = ConstantConfig.NEGATIVE_VALUE;
                        }
                        break;
                }
            }
            return productInInvoice;
        }

        // funtion try to convert string to date
        public static DateTime ConvertStringToDate(string date)
        {
            try
            {
                return DateTime.Parse(date);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // try to convert date to string, return bool
        public static bool TryConvertStringToDate(string date, out DateTime result)
        {
            try
            {
                result = DateTime.Parse(date);
                return true;
            }
            catch (Exception e)
            {
                result = DateTime.Now;
                Console.WriteLine(e.Message);
                return false;
            }
        }

        // try to convert string to long, return bool
        public static bool TryConvertStringToLong(string number, out long result)
        {
            try
            {
                result = (long)Convert.ToDouble(number);
                return true;
            }
            catch (Exception e)
            {
                result = ConstantConfig.NEGATIVE_VALUE;
                Console.WriteLine(e.Message);
                return false;
            }
        }

        internal static async Task<InvoiceCustom_VM_Scan> GetMoney(ByteString fileByteString, string fileMineType)
        {
            // create client
            var client = new DocumentProcessorServiceClientBuilder
            {
                Endpoint = $"{ConstantConfig.LOCATION}-documentai.googleapis.com"
            }.Build();

            // read file
            //var content = file.OpenReadStream();
            var rawDocument = new RawDocument
            {
                Content = fileByteString,
                MimeType = fileMineType
            };

            // Initialize request argument(s)
            var request = new ProcessRequest
            {
                Name = ProcessorName.FromProjectLocationProcessor(ConstantConfig.PROJECT_ID, ConstantConfig.LOCATION, ConstantConfig.PROCESSOR_ID).ToString(),
                RawDocument = rawDocument
            };

            // Make the request
            var response = await client.ProcessDocumentAsync(request);

            var invoiceDocumentAI = new InvoiceCustom_VM_Scan();
            foreach (var entity in response.Document.Entities)
            {
                switch (entity.Type)
                {
                    case "net_amount":
                        if (TryConvertStringToLong(entity.MentionText, out long resultNetAmount))
                        {
                            invoiceDocumentAI.NetAmount = resultNetAmount;
                        }
                        else
                        {
                            invoiceDocumentAI.NetAmount = ConstantConfig.DEFAULT_ZERO_VALUE;
                        }
                        break;
                    case "total_amount":
                        if (TryConvertStringToLong(entity.MentionText, out long resultTotalAmount))
                        {
                            invoiceDocumentAI.TotalAmount = resultTotalAmount;
                        }
                        else
                        {
                            invoiceDocumentAI.TotalAmount = ConstantConfig.DEFAULT_ZERO_VALUE;
                        }
                        break;
                    case "total_tax_amount":
                        if (TryConvertStringToLong(entity.MentionText, out long resultTaxAmount))
                        {
                            invoiceDocumentAI.TaxAmount = resultTaxAmount;
                        }
                        else
                        {
                            invoiceDocumentAI.TaxAmount = ConstantConfig.DEFAULT_ZERO_VALUE;
                        }
                        break;
                    case "supplier_name":
                        invoiceDocumentAI.SupplierName = entity.MentionText;
                        break;
                    case "supplier_address":
                        invoiceDocumentAI.SupplierAddress = entity.MentionText;
                        break;
                    case "supplier_phone":
                        invoiceDocumentAI.SupplierPhone = entity.MentionText;
                        break;
                }
            }
            return invoiceDocumentAI;
        }
    }
}
