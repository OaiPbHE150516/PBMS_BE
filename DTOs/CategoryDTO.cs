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

    public class CategoryDetail_VM_DTO : Category_VM_DTO
    {
        public bool IsRoot { get; set; }
        public int ParentCategoryID { get; set; }
        public virtual CategoryType CategoryType { get; set; } = null!;
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

    public class CategoryTree_VM_DTO
    {
        public int CategoryID { get; set; }
        public string NameVN { get; set; } = String.Empty;
        public int ParentCategoryID { get; set; }
        public bool IsRoot { get; set; }
        public virtual List<CategoryTree_VM_DTO> Children { get; set; } = new List<CategoryTree_VM_DTO>();
    }
}
