using AutoMapper;
using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Custom;
using pbms_be.Data.Filter;
using pbms_be.DataAccess;
using pbms_be.DTOs;

namespace pbms_be.Library
{
    public class DashboardCalculator
    {
        private readonly PbmsDbContext _context;

        public DashboardCalculator(PbmsDbContext context)
        {
            _context = context;
        }

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
