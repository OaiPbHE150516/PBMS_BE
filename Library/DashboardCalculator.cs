using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Custom;
using pbms_be.Data.Filter;
using pbms_be.DataAccess;
using pbms_be.DTOs;
using pbms_be.ThirdParty;

namespace pbms_be.Library
{
    public class DashboardCalculator
    {
        private readonly PbmsDbContext _context;

        public DashboardCalculator(PbmsDbContext context)
        {
            _context = context;
        }

        // some constant variable
        private const string INCOME = "income";
        private const string EXPENSE = "expense";
        private const int PERCENTAGE_COMPARE = 50;

        public object GetTotalAmountByCategory(int type, string accountID, DateTime fromDate, DateTime toDate, AutoMapper.IMapper? _mapper)
        {
            try
            {
                // type = 1: income, 2: expense
                var transDA = new TransactionDA(_context);
                var listTrans = transDA.GetTransactionsByDateTimeRange(accountID, fromDate, toDate);
                listTrans = type switch
                {
                    ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME => listTrans.Where(x => x.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME).ToList(),
                    ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_EXPENSE => listTrans.Where(x => x.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_EXPENSE).ToList(),
                    _ => throw new Exception(Message.VALUE_TYPE_IS_NOT_VALID),
                };
                var listTransByCategory = listTrans.GroupBy(x => x.CategoryID).ToList();
                // remove transaction that is not in the category type
                // new dictionary to store total amount of each category
                var result = new List<CategoryWithTransactionData>();
                long totalAmountOfRange = 0;
                var countCate = 1;
                foreach (var tran in listTransByCategory)
                {
                    var category = _context.Category
                                .Where(c => c.CategoryID == tran.Key)
                                .Include(c => c.CategoryType)
                                .FirstOrDefault() ?? throw new Exception(Message.CATEGORY_NOT_FOUND);
                    var totalAmount = tran.Sum(t => t.TotalAmount);
                    var totalAmountStr = LConvertVariable.ConvertToMoneyFormat(totalAmount);
                    var numberOfTransaction = tran.Count();
                    if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                    var CategoryWithAllTransaction = new CategoryWithTransactionData
                    {
                        CategoryNumber = countCate++,
                        Category = _mapper.Map<CategoryDetail_VM_DTO>(category),
                        TotalAmount = totalAmount,
                        TotalAmountStr = totalAmountStr,
                        NumberOfTransaction = numberOfTransaction
                    };
                    result.Add(CategoryWithAllTransaction);
                    totalAmountOfRange += totalAmount;
                }
                // foreach to calculate percentage of each category
                foreach (var item in result)
                {
                    // calculate percentage to 2 decimal places
                    item.Percentage = ((double)item.TotalAmount / totalAmountOfRange * 100);
                    item.PercentageStr = item.Percentage.ToString("0.00") + "%";
                }
                // sort list by total amount
                result.Sort((x, y) => y.TotalAmount.CompareTo(x.TotalAmount));
                return new
                {
                    TotalAmountOfRange = totalAmountOfRange,
                    TotalAmountOfRangeStr = LConvertVariable.ConvertToMoneyFormat(totalAmountOfRange),
                    TotalNumberOfTransaction = listTrans.Count,
                    TotalNumberOfCategory = listTransByCategory.Count,
                    CategoryWithTransactionData = result
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //var response = """ 
        //                                                {
        //                    "đồ ăn": [
        //                        "đồ ăn",
        //                        "Thực phẩm",
        //                        "food",
        //                        "snack",
        //                        "sữa",
        //                        "trái cây",
        //                        "đồ hộp",
        //                        "Bánh",
        //                        "Thịt",
        //                        "snack"
        //                    ],
        //                    "đồ dùng": [
        //                        "đồ dùng",
        //                        "Nong nan, tui (700g)",
        //                        "Khổ giấy",
        //                        "Vật dùng",
        //                        "Khăn giấy",
        //                        "Dụng cụ ăn uống"
        //                    ],
        //                    "đồ uống": [
        //                        "đồ uống",
        //                        "do uong",
        //                        "drink",
        //                        "Nuoc giai khat",
        //                        "Trà Sơt",
        //                        "Trà Sữa",
        //                        "Hồng trà",
        //                        "Cà phê",
        //                        "Nước tăng lực",
        //                        "Nước khoáng",
        //                        "Đồ uống đóng chai",
        //                        "Thực uống",
        //                        "Nước Chanh",
        //                        "Nước Cacao",
        //                        "Đồ uống đóng chai",
        //                        "THẠCH DỪA"
        //                    ],
        //                    "other": [
        //                        "Latte dao, chai (350ml)",
        //                        "CAFE",
        //                        "1",
        //                        "Sưả đẬ’c",
        //                        "Đồ án vật",
        //                        "Hướng Dương",
        //                        "Dịch vụ chuyển tiền",
        //                        "BÒ KOBE"
        //                    ]
        //                }
        //                """;

        internal object GetTotalAmountByTag(string accountID, DateTime fromDate, DateTime toDate, IMapper? mapper)
        {
            try
            {
                var transDA = new TransactionDA(_context);
                var listTrans = transDA.GetTransactionsByDateTimeRange(accountID, fromDate, toDate);
                // new list to store transactionID
                var listTransID = listTrans.Select(x => x.TransactionID).ToList();
                // select all invoice have transactionID in listTransID
                var listInvoice = _context.Invoice
                    .Where(i => listTransID.Contains(i.TransactionID))
                    .Include(i => i.ProductInInvoices)
                    .ToList();
                // new dictionary to store total amount of each tag
                // tag like 'food', 'food animal', 'food vegetable' to same group 'food'
                var listTagWithProductData = new List<TagWithProductData>();
                long totalAmountOfRange = 0;
                // get all product in invoice and group by tag
                var listProduct = listInvoice.SelectMany(x => x.ProductInInvoices).ToList();
                // remove product have tag is ""
                listProduct = listProduct.Where(x => x.Tag != "").ToList();
                var listTag = listProduct.Select(x => x.Tag).Distinct().ToList();
                var textprompt = _context.TextPrompt.Where(x => x.TextAction == "classify_tag").FirstOrDefault() ?? throw new Exception("not found text prompt");
                //// convert listTag to string json
                var listTagJson = JsonConvert.SerializeObject(listTag);
                // call api to classify tag
                var response = VertextAiMultimodalApi.ClassifyTag(textprompt.Text_Prompt, listTagJson);


                // convert response to dictioary list of tag
                var dictTagResponse = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>((string)response);
                if (dictTagResponse is null) throw new Exception("dictTagResponse is null");
                //return dictTagResponse;

                // push key and value of dictTagResponse to listTagWithProductData
                var countTag = 1;
                foreach ( var item in dictTagResponse)
                {
                    listTagWithProductData.Add(new TagWithProductData
                    {
                        TagNumber = countTag++,
                        Tag = new TagDetail_VM_DTO
                        {
                            PrimaryTag = item.Key,
                            ChildTags = item.Value,
                        }
                    });
                }

                foreach(var item in listTagWithProductData)
                {
                    // find all product in listProduct have same tag with item.Tag.PrimaryTag or item.Tag.ChildTags, then sum total amount
                    var totalAmount = listProduct.Where(x => item.Tag.ChildTags.Contains(x.Tag)).Sum(x => x.TotalAmount);
                    item.TotalAmount = totalAmount;
                    item.TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(totalAmount);
                    item.NumberOfProduct = listProduct.Count(x => item.Tag.ChildTags.Contains(x.Tag));
                    totalAmountOfRange += totalAmount;
                }

                // foreach to calculate percentage of each tag
                foreach (var item in listTagWithProductData)
                {
                    // calculate percentage to 2 decimal places
                    item.Percentage = ((double)item.TotalAmount / totalAmountOfRange * 100);
                    item.PercentageStr = item.Percentage.ToString("0.00") + "%";
                }

                // sort list by total amount
                listTagWithProductData.Sort((x, y) => y.TotalAmount.CompareTo(x.TotalAmount));

                // select all product have same tag to a list TagWithTotalAmount
                var listTagWithTotalAmount = new List<TagWithTotalAmount>();
                // group product in listProduct by tag
                var listProductGroupByTag = listProduct.GroupBy(x => x.Tag).ToList();
                foreach (var item in listProductGroupByTag)
                {
                    var totalAmount = item.Sum(x => x.TotalAmount);
                    var totalAmountStr = LConvertVariable.ConvertToMoneyFormat(totalAmount);
                    var numberOfProduct = item.Count();
                    listTagWithTotalAmount.Add(new TagWithTotalAmount
                    {
                        Tag = item.Key,
                        TotalAmount = totalAmount,
                        TotalAmountStr = totalAmountStr,
                        NumberOfProduct = numberOfProduct
                    });
                }
                // sort list by total amount
                listTagWithTotalAmount.Sort((x, y) => y.TotalAmount.CompareTo(x.TotalAmount));

                // loop through listTagWithProductData, if ChildTags contain tag in listTagWithTotalAmount, then add total amount of tag in listTagWithTotalAmount to total amount of tag in listTagWithProductData
                foreach (var item in listTagWithProductData)
                {
                    item.TagWithTotalAmounts = [];
                    foreach (var tag in item.Tag.ChildTags)
                    {
                        item.TagWithTotalAmounts.AddRange(listTagWithTotalAmount.Where(x => x.Tag == tag).ToList());
                    }
                    // add tagNumber from 1 to end of list TagWithTotalAmount
                    var countTagNumber = 1;
                    foreach (var tag in item.TagWithTotalAmounts)
                    {
                        tag.TagNumber = countTagNumber++;
                    }
                }

                return new
                {
                    TotalAmountOfRange = totalAmountOfRange,
                    TotalAmountOfRangeStr = LConvertVariable.ConvertToMoneyFormat(totalAmountOfRange),
                    TotalNumberOfTransaction = listTrans.Count,
                    TotalNumberOfProduct = listProduct.Count,
                    TotalNumberOfTag = listTagWithProductData.Count,
                    TagWithProductData = listTagWithProductData,
                    ListTag = listTag,
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // a function to compare 2 tag is contain each other or not, return true if tag1 contain tag2 or tag2 contain tag1
        private static bool IsContainTag(string tag1, string tag2)
        {
            // 1. Tính độ dài của tag ngắn hơn để sử dụng làm cơ sở so sánh
            int minLength = Math.Min(tag1.Length, tag2.Length);

            // 2. Số ký tự giống nhau
            int matchingCharacters = 0;

            // lower and remove diacritics
            tag1 = LConvertVariable.RemoveDiacritics(tag1.ToLower());
            tag2 = LConvertVariable.RemoveDiacritics(tag2.ToLower());

            // 3. Tính số ký tự giống nhau
            for (int i = 0; i < minLength; i++)
            {
                if (tag1[i] == tag2[i])
                {
                    matchingCharacters++;
                }
            }

            // 4. Tính phần trăm tương đồng
            double similarityPercentage = (double)matchingCharacters / minLength * 100;

            // 5. So sánh phần trăm tương đồng với ngưỡng
            return similarityPercentage > 30;
        }

        internal object GetTotalAmountByType(string accountID, DateTime fromDate, DateTime toDate, IMapper? _mapper)
        {
            try
            {
                var transDA = new TransactionDA(_context);
                var listTrans = transDA.GetTransactionsByDateTimeRange(accountID, fromDate, toDate);
                var listTransByType = listTrans.GroupBy(x => x.Category.CategoryTypeID).ToList();
                // new dictionary to store total amount of each category
                var result = new List<CategoryWithTransactionData2>();
                long totalAmountOfRange = 0;
                var countType = 1;
                foreach (var tran in listTransByType)
                {
                    var categoryType = _context.CategoryType
                                .Where(c => c.CategoryTypeID == tran.Key)
                                .FirstOrDefault() ?? throw new Exception(Message.CATEGORY_NOT_FOUND);
                    var totalAmount = tran.Sum(t => t.TotalAmount);
                    var totalAmountStr = LConvertVariable.ConvertToMoneyFormat(totalAmount);
                    var numberOfTransaction = tran.Count();
                    if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                    var CategoryTypeWithAllTransaction = new CategoryWithTransactionData2
                    {
                        CategoryTypeNumber = countType++,
                        CategoryType = categoryType,
                        TotalAmount = totalAmount,
                        TotalAmountStr = totalAmountStr,
                        NumberOfTransaction = numberOfTransaction
                    };
                    result.Add(CategoryTypeWithAllTransaction);
                    totalAmountOfRange += totalAmount;
                }
                // foreach to calculate percentage of each category
                foreach (var item in result)
                {
                    // calculate percentage to 2 decimal places
                    item.Percentage = ((double)item.TotalAmount / totalAmountOfRange * 100);
                    item.PercentageStr = item.Percentage.ToString("0.00") + "%";
                }
                // sort result by total amount
                result = [.. result.OrderByDescending(x => x.TotalAmount)];

                var result2 = result.OrderBy(x => x.CategoryType.CategoryTypeID).ToList();
                return new
                {
                    TotalAmountOfRange = totalAmountOfRange,
                    TotalAmountOfRangeStr = LConvertVariable.ConvertToMoneyFormat(totalAmountOfRange),
                    MinusAmountOfRange = result2[0].TotalAmount - result2[^1].TotalAmount,
                    MinusAmountOfRangeStr = LConvertVariable.ConvertToMoneyFormat(result2[0].TotalAmount - result2[^1].TotalAmount),
                    TotalNumberOfTransaction = listTrans.Count,
                    TotalNumberOfCategory = listTransByType.Count,
                    CategoryWithTransactionData = result
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
