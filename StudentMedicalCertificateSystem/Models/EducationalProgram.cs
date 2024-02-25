using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.Models
{
    public class EducationalProgram
    {
        [Key]
        public int ProgramID { get; set; }
        [Required(ErrorMessage = "Необходимо ввести наименование программы")]
        public string? ProgramName { get; set; }
    }
}
