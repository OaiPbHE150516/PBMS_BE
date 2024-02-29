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
       public int BudgetCategory {  get; set; }
       public float BudgetAmount { get; set; }
       public int WalletID { get; set; }
       public DateTime FirstOfMonth { get; set; }
       public DateTime FirstOfWeek { get; set; }
       public int RepeatInterval { get; set; }
        public string Note { get; set; } = string.Empty;
    }
    //Update Budget
    public class UpdateBudgetDTO
    {
        public int BudgetID { get; set; } 
        public string BudgetName { get; set; } = string.Empty;
        public int BudgetCategory { get; set; }
        public float BudgetAmount { get; set; }
        public int WalletID { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
