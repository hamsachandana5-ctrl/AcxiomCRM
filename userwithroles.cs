using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.ViewModels
{
    public class UserWithRolesViewModel
    {
        public IdentityUser User { get; set; } = new();
        public IList<string> Roles { get; set; } = new List<string>();
    }
}