using Google.Apis.Storage.v1.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using pbms_be.Data.Budget;
using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using pbms_be.Data.Filter;
using pbms_be.DataAccess;
using System.Text;

namespace pbms_be.Data.Budget;

[Table("budgetcategory", Schema = "public")]

public class BudgetCategory
{
    /*
        CREATE TABLE budgetcategory(
            budgetcategory_id serial PRIMARY KEY,
            budget_id INT NOT NULL,
            category_id INT NOT NULL,
            as_id int NOT NULL DEFAULT 1,
            FOREIGN KEY(budget_id) REFERENCES budget(budget_id),
            FOREIGN KEY(category_id) REFERENCES category(category_id),
            FOREIGN KEY(as_id) REFERENCES active_state(as_id)
        );
    */

    [Column("budgetcategory_id")]
    [Key]
    public int BudgetCategoryID { get; set; }

    [Column("budget_id")]
    public int BudgetID { get; set; }

    [Column("category_id")]
    public int CategoryID { get; set; }

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;
}
