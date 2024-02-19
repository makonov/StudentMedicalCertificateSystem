using StudentMedicalCertificateSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class CreateMedicalCertificateViewModel
    {
        [Required(ErrorMessage = "Необходимо выбрать студента")]
        public string FullName { get; set; }
        public int? StudentID { get; set; }
        [Required(ErrorMessage = "Необходимо выбрать больницу")]
        public int? ClinicID { get; set; }
        [Required(ErrorMessage = "Необходимо выбрать диагноз")]
        public int? DiagnosisID { get; set; }
        public string? CertificatePath { get; set; }
        [Required(ErrorMessage = "Необходимо выбрать дату начала болезни"), DataType(DataType.Date)]
        public DateTime? IlnessDate { get; set; }
        [Required(ErrorMessage = "Необходимо выьрать дату конца болезни"), DataType(DataType.Date)]
        public DateTime? RecoveryDate { get; set; }
        public string? Answer { get; set; }
    }
}
