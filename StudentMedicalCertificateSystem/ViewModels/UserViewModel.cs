namespace StudentMedicalCertificateSystem.ViewModels
{
    public class UserViewModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public IList<string> Roles { get; set; }
    }
}
