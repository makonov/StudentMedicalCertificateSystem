using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMedicalCertificateSystem.Models
{
    public class User : IdentityUser
    {
        [ForeignKey("EducationalOffice")]
        public int OfficeID { get; set; }

        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Patronymic {  get; set; }

    }
}
