using StudentMedicalCertificateSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class CreateMedicalCertificateViewModel
    {
        public string FullName { get; set; }
        [Required(ErrorMessage = "ID студента обязателен")]
        public int StudentID { get; set; }
        [Required(ErrorMessage = "ID клиники обязателен")]
        public int ClinicID { get; set; }
        [Required(ErrorMessage = "ID диагноза обязателен")]
        public int DiagnosisID { get; set; }
        public string? CertificatePath { get; set; }
        [Required(ErrorMessage = "Дата выздоровления обязательна"), DataType(DataType.Date)]
        public DateTime? IlnessDate { get; set; }
        [Required(ErrorMessage = "Дата выздоровления обязательна"), DataType(DataType.Date)]
        public DateTime? RecoveryDate { get; set; }
        public string? Answer { get; set; }
    }
}
