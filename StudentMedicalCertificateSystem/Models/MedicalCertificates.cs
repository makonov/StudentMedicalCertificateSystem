using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMedicalCertificateSystem.Models
{
    public class MedicalCertificates
    {
        [Key]
        public int CertificateID { get; set; }

        [ForeignKey("Students")]
        [Required(ErrorMessage = "ID студента обязателен")]
        public int StudentID { get; set; }
        public virtual Students? Student { get; set; }

        [ForeignKey("Users")]
        [Required(ErrorMessage = "ID ответственного работника обязателен")]
        public int UserID { get; set; }
        public virtual Users? User { get; set; }

        [ForeignKey("Clinics")]
        [Required(ErrorMessage = "ID клиники обязателен")]
        public int ClinicID { get; set; }
        public virtual Clinics? Clinic { get; set; }

        [ForeignKey("Diagnoses")]
        [Required(ErrorMessage = "ID диагноза обязателен")]
        public int DiagnosisID { get; set; }
        public virtual Diagnoses? Diagnosis { get; set; }

        public string? CertificatePath { get; set; }

        [Required(ErrorMessage = "Дата начала болезни обязательна"), DataType(DataType.Date)]
        public DateTime IlnessDate { get; set; }

        [Required(ErrorMessage = "Дата выздоровления обязательна"), DataType(DataType.Date)]
        public DateTime RecoveryDate { get; set; }

        public string? Answer {  get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; }
    }
}
