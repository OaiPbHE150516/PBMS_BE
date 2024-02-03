﻿using Google.Cloud.DocumentAI.V1;
using Google.Protobuf;
using Google.Protobuf.Collections;
using pbms_be.Configurations;
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
                foreach (var entity in entities)
                {
                    if (entity.Properties.Count > 0)
                    {
                        invoice.ProductInInvoices.Add(GetProductInInvoice(entity.Properties));
                        continue;
                    }
                    var type = entity.Type;
                    switch (entity.Type)
                    {
                        case ConstantConfig.CURRENCY:
                            //invoice.CurrencyID = entity.MentionText;
                            invoice.CurrencyID = 2;
                            break;
                        case ConstantConfig.INVOICE_ID:
                            // convert to int
                            invoice.InvoiceID = int.Parse(entity.MentionText);
                            break;
                        case ConstantConfig.INVOICE_DATE:
                            //var invoicedate = ConvertStringToDate(entity.MentionText);
                            // use TryConvertStringToDate to check if the date is valid
                            if (TryConvertStringToDate(entity.MentionText, out DateTime result))
                            {
                                invoice.InvoiceDate = result;
                            }
                            break;
                        case ConstantConfig.INVOICE_DISCOUNT:
                            invoice.Discount = long.Parse(entity.MentionText);
                            break;
                        case ConstantConfig.INVOICE_NOTE:
                            invoice.Note = entity.MentionText;
                            break;
                        case ConstantConfig.NET_AMOUNT:
                            invoice.NetAmount = long.Parse(entity.MentionText);
                            break;
                        case ConstantConfig.PAYMENT_TERMS:
                            invoice.PaymentTerms = entity.MentionText;
                            break;
                        case ConstantConfig.SHIP_TO_ADDRESS:
                            invoice.ReceiverAddress = entity.MentionText;
                            break;
                        case ConstantConfig.SHIP_TO_NAME:
                            invoice.ReceiverName = entity.MentionText;
                            break;
                        case ConstantConfig.SUPPLIER_ADDRESS:
                            invoice.SupplierAddress = entity.MentionText;
                            break;
                        case ConstantConfig.SUPPLIER_EMAIL:
                            invoice.SupplierEmail = entity.MentionText;
                            break;
                        case ConstantConfig.SUPPLIER_NAME:
                            invoice.SupplierName = entity.MentionText;
                            break;
                        case ConstantConfig.SUPPLIER_PHONE:
                            invoice.SupplierPhone = entity.MentionText;
                            break;
                        //case ConstantConfig.SUPPLIER_WEBSITE:
                        //    invoice.SupplierWebsite = entity.MentionText;
                        //    break;
                        case ConstantConfig.TOTAL_AMOUNT:
                            invoice.TotalAmount = long.Parse(entity.MentionText);
                            break;
                        case ConstantConfig.TOTAL_TAX_AMOUNT:
                            invoice.TaxAmount = long.Parse(entity.MentionText);
                            break;
                    }
                    // end of switch case
                }
                // end of loop
                invoice.InvoiceImageURL = "url";
                invoice.InvoiceRawDatalog = entities.ToString();
                invoice.ActiveStateID = 1;
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
                    case ConstantConfig.LINE_ITEM_DESCRIPTION:
                        productInInvoice.ProductName = property.MentionText;
                        break;
                    case ConstantConfig.LINE_ITEM_QUANTITY:
                        productInInvoice.Quanity = int.Parse(property.MentionText);
                        break;
                    case ConstantConfig.LINE_ITEM_UNIT_PRICE:
                        productInInvoice.UnitPrice = long.Parse(property.MentionText);
                        break;
                    case ConstantConfig.LINE_ITEM_AMOUNT:
                        productInInvoice.TotalAmount = long.Parse(property.MentionText);
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
                return false;
            }
        }

    }
}
