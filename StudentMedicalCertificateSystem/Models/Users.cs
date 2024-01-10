using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMedicalCertificateSystem.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }
        [ForeignKey("EducationalOffices")]
        public int OfficeID { get; set; }
        public bool IsAdmin { get; set; }
        public string? UserLogin { get; set; }
        public string? UserPassword { get; set; }

        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Patronymic {  get; set; }

    }
}
