using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Budget;
using pbms_be.Data.CollabFund;
using pbms_be.Data.Filter;
using pbms_be.Data.WalletF;
using pbms_be.DTOs;


namespace pbms_be.DataAccess
{
    public class BudgetDA
    {
        private readonly PbmsDbContext _context;
        private readonly List<BudgetType> _budgetTypes;
        public BudgetDA(PbmsDbContext context)
        {
            _context = context;
            _budgetTypes = new List<BudgetType>
            {
                new BudgetType { BudgetTypeID = 0, TypeName = "Other" },
                new BudgetType { BudgetTypeID = 1, TypeName = "Week" },
                new BudgetType { BudgetTypeID = 2, TypeName = "Month" }
            };
        }
        //GetBudget by ID
        internal Budget GetBudget(int budgetID)
        {
            try
            {
                var result = _context.Budget
                            .Where(x => x.BudgetID == budgetID && x.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(x => x.ActiveState)
                            .FirstOrDefault();
                if (result is null) throw new Exception(Message.BUDGET_NOT_FOUND);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Get all budget
        internal List<BudgetWithCategoryDTO> GetBudgets(string accountID, AutoMapper.IMapper _mapper)
        {
            try
            {

                var result = _context.Budget
                            .Where(x => x.AccountID == accountID
                                         && x.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(x => x.ActiveState)
                            .ToList();
                foreach (var item in result)
                {
                    var btype = _budgetTypes.Find(x => x.BudgetTypeID == item.BudgetTypeID) ?? throw new Exception();
                    item.BudgetType = btype;
                }
                var listBudgetDTO = _mapper.Map<List<BudgetWithCategoryDTO>>(result);
                foreach (var item in listBudgetDTO)
                {
                    //var categoriesResult = new List<Category>();
                    //var budget = new Budget();
                    //GetBudgetDetail(accountID, item.BudgetID, out categoriesResult, out budget);
                    //item.Categories = categoriesResult;
                    var budgetcategories = _context.BudgetCategory
                                    .Where(x => x.BudgetID == item.BudgetID && x.ActiveStateID == ActiveStateConst.ACTIVE)
                                    .Include(x => x.ActiveState)
                                    .ToList();
                    var categories = new List<Category>();
                    foreach (var bc in budgetcategories)
                    {
                        var cate = _context.Category
                                    .Where(x => x.CategoryID == bc.CategoryID && x.ActiveStateID == ActiveStateConst.ACTIVE)
                                    .Include(x => x.ActiveState)
                                    .FirstOrDefault();
                        if (cate is null) continue;
                        categories.Add(cate);
                    }
                    item.Categories = categories;
                }
                return listBudgetDTO;
                //return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        internal void GetBudgetDetail(string accountID, int budgetID, out List<Category> categoriesResult, out Budget budget)
        {
            try
            {
                // get budget by budget id and account id
                var result = _context.Budget
                            .Where(x => x.BudgetID == budgetID && x.AccountID == accountID && x.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(x => x.ActiveState)
                            .FirstOrDefault();
                if (result is null) throw new Exception(Message.BUDGET_NOT_FOUND);
                var btype = _budgetTypes.Find(x => x.BudgetTypeID == result.BudgetTypeID);
                if (btype is null) throw new Exception();
                result.BudgetType = btype;
                // get all category by budget id
                var categories = _context.BudgetCategory
                                .Where(x => x.BudgetID == budgetID && x.ActiveStateID == ActiveStateConst.ACTIVE)
                                .Include(x => x.ActiveState)
                                .ToList();
                if (categories.Count == ConstantConfig.DEFAULT_ZERO_VALUE) throw new Exception(Message.CATEGORY_NOT_FOUND);
                var categoryList = new List<Category>();
                foreach (var category in categories)
                {
                    // get category by category id
                    var cate = _context.Category
                                .Where(x => x.CategoryID == category.CategoryID && x.ActiveStateID == ActiveStateConst.ACTIVE)
                                .Include(x => x.ActiveState)
                                .FirstOrDefault();
                    if (cate is null) throw new Exception(Message.CATEGORY_NOT_FOUND);
                    categoryList.Add(cate);
                }
                // add categories to result
                //result.Categories = categoryList;
                categoriesResult = categoryList;
                budget = result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        //Create Budget

        internal Budget CreateBudget(CreateBudgetDTO budgetDTO)
        {
            try
            {
                var budget = new Budget
                {
                    AccountID = budgetDTO.AccountID,
                    BudgetName = budgetDTO.BudgetName,
                    TargetAmount = budgetDTO.TargetAmount,
                    BeginDate = budgetDTO.BeginDate,
                    EndDate = budgetDTO.EndDate,
                    BudgetTypeID = budgetDTO.BudgetTypeID,
                    RepeatInterVal = budgetDTO.RepeatInterVal,
                    Note = budgetDTO.Note,
                    CreateTime = DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC).ToUniversalTime(),
                    ActiveStateID = ActiveStateConst.ACTIVE
                };
                budget.ActiveStateID = 1;
                _context.Budget.Add(budget);
                _context.SaveChanges();

                var result = GetBudget(budget.BudgetID);
                var listBudgetCategory = new List<BudgetCategory>();

                foreach (var categoryID in budgetDTO.CategoryIDs)
                {
                    var budgetCategory = new BudgetCategory
                    {
                        BudgetID = result.BudgetID,
                        CategoryID = categoryID,
                        ActiveStateID = ActiveStateConst.ACTIVE
                    };
                    listBudgetCategory.Add(budgetCategory);
                }
                _context.BudgetCategory.AddRange(listBudgetCategory);
                _context.SaveChanges();

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        // Check if budget exist by name
        public bool IsBudgetExist(Budget budget)
        {
            var exist = _context.Budget.Any(x => x.BudgetName == budget.BudgetName);
            return exist;
        }

        public bool IsBudgetExist(string accountID, int budgetID)
        {
            var exist = _context.Budget.Any(x => x.BudgetID == budgetID && x.AccountID == accountID);
            return exist;
        }

        // Update Budget
        public Budget? UpdateBudget(Budget budget)
        {
            if (IsBudgetExist(budget))
            {
                _context.Budget.Update(budget);
                _context.SaveChanges();
                return GetBudget(budget.BudgetID);
            }
            return null;
        }

        internal object? GetBudgetType()
        {
            try
            {
                return _budgetTypes;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal bool IsBudgetTypeExist(int budgetTypeID)
        {
            var exist = _budgetTypes.Any(x => x.BudgetTypeID == budgetTypeID);
            return exist;
        }

        internal object DeleteBudget(DeleteBudgetDTO budget, AutoMapper.IMapper? _mapper)
        {
            try
            {
                var result = GetBudget(budget.BudgetID, budget.AccountID) ?? throw new Exception(Message.BUDGET_NOT_FOUND);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                if (result.AccountID != budget.AccountID) throw new Exception(Message.BUDGET_NOT_FOUND);
                result.ActiveStateID = ActiveStateConst.DELETED;
                _context.SaveChanges();
                return GetBudgets(budget.AccountID, _mapper);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Budget GetBudget(int budgetID, string accountID)
        {
            try
            {
                var result = _context.Budget
                        .Where(x => x.BudgetID == budgetID && x.AccountID == accountID && x.ActiveStateID == ActiveStateConst.ACTIVE)
                        .Include(x => x.ActiveState)
                        .FirstOrDefault();
                if (result is null) throw new Exception(Message.BUDGET_NOT_FOUND);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // add amount to current amount of budget by budget id, account id and amount
        public Budget AddAmountToBudget(int budgetID, string accountID, long amount)
        {
            try
            {
                var budget = GetBudget(budgetID, accountID);
                budget.CurrentAmount += amount;
                _context.SaveChanges();
                return budget;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // function check that budget is exist by budget id and account id
        public bool IsBudgetExist(int budgetID, string accountID)
        {
            var exist = _context.Budget.Any(x => x.BudgetID == budgetID && x.AccountID == accountID);
            return exist;
        }

        internal void UpdateBudgetAmount(string accountID, long plusamount, int categoryID)
        {
            try
            {
                if (plusamount == ConstantConfig.DEFAULT_ZERO_VALUE) return;
                // 1. find budgets by account id that have category id, join with budget category
                var budgets = _context.Budget
                                .Join(_context.BudgetCategory,
                                    b => b.BudgetID,
                                    bc => bc.BudgetID,
                                    (b, bc) => new { b, bc })
                                .Where(x => x.b.AccountID == accountID
                                    && x.bc.CategoryID == categoryID
                                    && x.b.ActiveStateID == ActiveStateConst.ACTIVE)
                                .Select(x => x.b)
                                .ToList();
                if (budgets.Count == ConstantConfig.DEFAULT_ZERO_VALUE) return;
                // 2. update current amount of budget in budgets by add plusamount to current amount
                foreach (var budget in budgets)
                {
                    budget.CurrentAmount += plusamount;
                }
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
