using Google.Apis.Storage.v1.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using pbms_be.Data.Budget;
using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace pbms_be.Data.Budget
{
    [Table("budgetcategory", Schema = "public")]

    public class BudgetCategory
    {

        //      CREATE TABLE budgetcategory(
        //      budgetCategory_id serial PRIMARY KEY,
        //      budget_id int NOT NULL,
        //      category_id int NOT NULL,
        //      FOREIGN KEY(budget_id) REFERENCES budget(budget_id),
        //      FOREIGN KEY(category_id) REFERENCES category(category_id)
        [Column("budgetCategory_id")]
        [Key]
        public int BudgetCategoryID { get; set; }

        [Column("budget_id")]
        public int BudgetID { get; set; }

        [Column("category_id")]
        public int CategoryID { get; set; }
    }
}
