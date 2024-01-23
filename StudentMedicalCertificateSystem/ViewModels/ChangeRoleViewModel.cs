using Microsoft.AspNetCore.Identity;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
