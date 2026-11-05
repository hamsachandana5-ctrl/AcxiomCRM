using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; } = "";

        public string Email { get; set; } = "";

        public List<string> AvailableRoles { get; set; } = new();

        [Required]
        public List<string> SelectedRoles { get; set; } = new();
    }
}