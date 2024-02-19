using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.Models
{
    public class Clinic
    {
        [Key]
        public int ClinicID { get; set; }
        [Required(ErrorMessage = "Необходимо ввести наименование больницы")]
        public string? ClinicName { get; set; }
    }
}
