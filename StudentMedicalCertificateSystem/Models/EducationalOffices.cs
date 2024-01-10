using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.Models
{
    public class EducationalOffices
    {
        [Key]
        public int OfficeID { get; set; }
        public string? OfficeName { get; set; }
    }
}
