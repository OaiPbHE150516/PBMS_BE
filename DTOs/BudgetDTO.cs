namespace pbms_be.DTOs
{
    public class BudgetDTO
    {

    }
    //Create new budget
    public class CreateBudgetDTO
    {
       public string AccountID { get; set; } = string.Empty;
       public string BudgetName { get; set; } = string.Empty;
       public long BudgetAmount { get; set; }
       public string Note { get; set; } = string.Empty;

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
