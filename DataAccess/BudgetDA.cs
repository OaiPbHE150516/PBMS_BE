using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Budget;
using pbms_be.Data.CollabFund;
using pbms_be.Data.WalletF;


namespace pbms_be.DataAccess
{
    public class BudgetDA
    {
        private readonly PbmsDbContext _context;
        public BudgetDA(PbmsDbContext context)
        {
            _context = context;
        }
        //GetBudget by ID
        internal Budget GetBudget(int budgetID)
        {
            try
            {
                var result = _context.Budget
                            .Where(x => x.BudgetID == budgetID)
                            .Include(x => x.WalletID)
                            .Include(x => x.BudgetCategoryID)
                            .FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        internal Budget GetBudget(int budgetID, int categoryID)
        {
            try
            {
                var budgetCate = _context.BudgetCategory
                                    .Where(x => x.CategoryID == categoryID)
                                    .Select(x => x.BudgetCategoryID)
                                    .ToList();

                var result = _context.Budget
                            .Where(x => budgetCate.Contains(x.BudgetID)
                                    && x.BudgetID == budgetID )
                            .Include(x => x.WalletID)
                            .FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        //Get all budget
        internal List<Budget> GetBudget(string accountID, int budgetID)
        {
            try
            {
                var budgetCate = _context.BudgetCategory
                                    .Where(x => x.BudgetCategoryID == budgetID)
                                    .Select(x => x.BudgetCategoryID)
                                    .ToList();

                var result = _context.Budget
                            .Where(x => budgetCate.Contains(x.BudgetID)
                                        && x.AccountID == accountID)
                            .Include(x => x.WalletID)
                            .ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        // Create Budget

        internal Budget CreateBudget(Budget budget)
        {
            try
            {
                if (IsBudgetExist(budget))
                {
                    throw new Exception(Message.Budget_ALREADY_EXIST);
                }
                _context.Budget.Add(budget);
                _context.SaveChanges();
                return GetBudget(budget.BudgetID);
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
    }
}
