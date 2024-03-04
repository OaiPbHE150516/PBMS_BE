using pbms_be.Data.Filter;

namespace pbms_be.DTOs
{
    public class CategoryDTO
    {

    }

    public class Category_VM_DTO
    {
        // CategoryID, NameVN, 
        public int CategoryID { get; set; }
        public string NameVN { get; set; } = String.Empty;
    }

    public class CategoryInTransaction_VM_DTO
    {
        public string NameVN { get; set; } = String.Empty;
        public string NameEN { get; set; } = String.Empty;
        public int CategoryTypeID { get; set; }
        public virtual CategoryType CategoryType { get; set; } = null!;
        public bool IsRoot { get; set; }
    }

    public class CategoryCreateDTO
    {
        public string AccountID { get; set; } = String.Empty;
        public string NameVN { get; set; } = String.Empty;
        public string NameEN { get; set; } = String.Empty;
        public int ParentCategoryID { get; set; }
    }

    // CategoryUpdateDTO
    public class CategoryUpdateDTO
    {
        public int CategoryID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public string NameVN { get; set; } = String.Empty;
        public string NameEN { get; set; } = String.Empty;
        public int ParentCategoryID { get; set; }
    }
}
