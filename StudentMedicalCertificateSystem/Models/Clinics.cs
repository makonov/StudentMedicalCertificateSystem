using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.Models
{
    public class Clinics
    {
        [Key]
        public int ClinicID { get; set; }
        public string? ClinicName { get; set; }
    }
}
