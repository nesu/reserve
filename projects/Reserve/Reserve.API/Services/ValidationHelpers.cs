using System.Linq;
using Reserve.Data;

namespace Reserve.API.Services
{
    public class ValidationHelpers
    {
        private readonly DatabaseContext _context;

        public ValidationHelpers(DatabaseContext context)
        {
            _context = context;
        }
        
        public bool ServiceAssigneeExists(long id)
        {
            return _context.ServiceAssignees.Any(it => it.Id == id);
        }

        public bool ServiceLocationExists(long location_id)
        {
            return _context.ServiceLocations.Any(it => it.Id == location_id);
        }
        
        public bool ServiceExists(long service_id)
        {
            return _context.Services.Any(it => it.Id == service_id);
        }
        
        public bool CategoryLabelExists(string category_label)
        {
            return _context.ServiceCategories.Any(it => it.Label == category_label);
        }

        public bool CategoryExists(long service_id)
        {
            return _context.ServiceCategories.Any(it => it.Id == service_id);
        }

        public bool ParentCategoryExists(long? parent_category_id)
        {
            if (!parent_category_id.HasValue)
                return false;
            
            return _context.ServiceCategories.Any(it => it.Id == parent_category_id.Value);
        }

        public bool AccountExists(long id)
        {
            return _context.Accounts.Any(it => it.Id == id);
        }

        public bool IsAccountUnique(string email)
        {
            return !_context.Accounts.Any(it => it.Email == email);
        }

        public bool IsPageUnique(string identifier)
        {
            return !_context.Pages.Any(it => it.Identifier == identifier);
        }
    }
}