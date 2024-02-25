using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks.Dataflow;

namespace StudentMedicalCertificateSystem.Models
{
    public class StudentGroup
    {
        [Key]
        public int GroupID { get; set; }
        [ForeignKey("EducationalProgram")]
        [Required(ErrorMessage = "Необходимо выбрать образовательную программу")]
        public int? ProgramID {  get; set; }
        public virtual EducationalProgram? Program { get; set; }
        [Required(ErrorMessage = "Необходимо ввести наименование группы")]
        [RegularExpression(@"^[А-ЯA-Zа-яa-z]+-\d{2}-\d+$", ErrorMessage = "Неверный формат наименования группы")]
        public string? GroupName { get; set; }

    }
}
