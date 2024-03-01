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
        public int StudentID { get; set; }
        public virtual Student? Student { get; set; }

        [ForeignKey("User")]
        public string? UserID { get; set; }
        public virtual User? User { get; set; }

        [ForeignKey("Clinic")]
        public int ClinicID { get; set; }
        public virtual Clinic? Clinic { get; set; }

        [ForeignKey("Diagnosis")]
        public int DiagnosisID { get; set; }
        public virtual Diagnosis? Diagnosis { get; set; }

        public int? CertificateNumber { get; set; }
        public string? CertificatePath { get; set; }
        public string? ClinicAnswerPath { get; set; }
        public bool IsConfirmed { get; set; }
        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? IllnessDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RecoveryDate { get; set; }

        public string? Answer {  get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
