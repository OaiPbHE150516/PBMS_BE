using Microsoft.EntityFrameworkCore;
using pbms_be.Data;
using pbms_be.Data.Filter;

namespace pbms_be.DataAccess
{
    public class CategoryDA
    {
        private readonly PbmsDbContext _context;

        public CategoryDA(PbmsDbContext context)
        {
            _context = context;
        }

        // get all category by account id
        public List<Category> GetCategories(string AccountID)
        {
            var result = _context.Category
                .Where(c => c.AccountID == AccountID)
                .Include(c => c.ActiveState)
                .ToList();
            return result;
        }

        // add a new category
    }
}
