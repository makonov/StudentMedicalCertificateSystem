using StudentMedicalCertificateSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class EditMedicalCertificateViewModel
    {
        public int CertificateID { get; set; }
        public bool IsConfirmed { get; set; }
        [Required(ErrorMessage = "Необходимо выбрать студента")]
        public int? StudentID { get; set; }
        public string? FullName { get; set; }
        public string? ImagePath { get; set; }
        public string? ClinicAnswerPath { get; set; }
        [Required(ErrorMessage = "Необходимо выбрать больницу")]
        public int? ClinicID { get; set; }
        [Required(ErrorMessage = "Необходимо выбрать диагноз")]
        public int? DiagnosisID { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? ClinicAnswer { get; set; }
        public int? CertificateNumber { get; set; }
        [Required(ErrorMessage = "Дата выдачи справки обязательна"), DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }
        [Required(ErrorMessage = "Дата выздоровления обязательна"), DataType(DataType.Date)]
        public DateTime? IlnessDate { get; set; }
        [Required(ErrorMessage = "Дата выздоровления обязательна"), DataType(DataType.Date)]
        public DateTime? RecoveryDate { get; set; }
        public string? Answer { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
