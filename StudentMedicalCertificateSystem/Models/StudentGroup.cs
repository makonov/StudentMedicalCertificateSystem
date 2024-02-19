using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks.Dataflow;

namespace StudentMedicalCertificateSystem.Models
{
    public class StudentGroup
    {
        [Key]
        public int GroupID { get; set; }
        [Required(ErrorMessage = "Необходимо ввести наименование группы")]
        [RegularExpression(@"^[А-ЯA-Zа-яa-z]+-\d{2}-\d+$", ErrorMessage = "Неверный формат наименования группы")]
        public string? GroupName { get; set; }
    }
}
