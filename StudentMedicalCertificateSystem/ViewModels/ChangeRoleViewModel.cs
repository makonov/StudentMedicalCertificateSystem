using Microsoft.AspNetCore.Identity;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public string UserRole { get; set; }
        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
        }
    }
}
