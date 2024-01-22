using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.Models
{
    public class Diagnosis
    {
        [Key]
        public int DiagnosisID { get; set; }
        public string? DiagnosisName { get; set; }
    }
}
