using pbms_be.Data.Filter;
using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations;

namespace pbms_be.DTOs
{
    public class BudgetDTO
    {

    }

    //BudgetWithCategory
    public class BudgetWithCategoryDTO
    {
        public int BudgetID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public string BudgetName { get; set; } = string.Empty;
        public long BudgetAmount { get; set; }
        public long CurrentAmount { get; set; }
        public DateTime BeginDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public int RepeatInterVal { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public int ActiveStateID { get; set; }
        public virtual ActiveState ActiveState { get; set; } = null!;
        public virtual List<Category> Categories { get; set; } = null!;
    }

    //Create new budget
    public class CreateBudgetDTO
    {
        public string AccountID { get; set; } = string.Empty;
        public string BudgetName { get; set; } = string.Empty;
        public long TargetAmount { get; set; }
        public DateTime BeginDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public int RepeatInterVal { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public List<int> CategoryIDs { get; set; } = null!;

    }
    //Update Budget
    public class UpdateBudgetDTO
    {
        public int BudgetID { get; set; }
        public string BudgetName { get; set; } = string.Empty;
        public long BudgetAmount { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
