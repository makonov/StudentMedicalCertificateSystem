using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.Models
{
    public class EducationalOffice
    {
        [Key]
        public int OfficeID { get; set; }
        [Required(ErrorMessage = "Необходимо ввести наименование учебного офиса")]
        public string? OfficeName { get; set; }
    }
}
