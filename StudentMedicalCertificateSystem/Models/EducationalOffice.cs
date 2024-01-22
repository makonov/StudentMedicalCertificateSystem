using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.Models
{
    public class EducationalOffice
    {
        [Key]
        public int OfficeID { get; set; }
        public string? OfficeName { get; set; }
    }
}
