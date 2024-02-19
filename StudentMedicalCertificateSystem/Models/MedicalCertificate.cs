using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMedicalCertificateSystem.Models
{
    public class MedicalCertificate
    {
        [Key]
        public int CertificateID { get; set; }

        [ForeignKey("Student")]
        [Required(ErrorMessage = "ID студента обязателен")]
        public int StudentID { get; set; }
        public virtual Student? Student { get; set; }

        [ForeignKey("Clinic")]
        [Required(ErrorMessage = "ID клиники обязателен")]
        public int ClinicID { get; set; }
        public virtual Clinic? Clinic { get; set; }

        [ForeignKey("Diagnosis")]
        [Required(ErrorMessage = "ID диагноза обязателен")]
        public int DiagnosisID { get; set; }
        public virtual Diagnosis? Diagnosis { get; set; }

        public string? CertificatePath { get; set; }

        [Required(ErrorMessage = "Дата начала болезни обязательна"), DataType(DataType.Date)]
        public DateTime IllnessDate { get; set; }

        [Required(ErrorMessage = "Дата выздоровления обязательна"), DataType(DataType.Date)]
        public DateTime RecoveryDate { get; set; }

        public string? Answer {  get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
