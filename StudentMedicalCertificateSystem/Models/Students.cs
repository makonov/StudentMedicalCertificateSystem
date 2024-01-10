using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMedicalCertificateSystem.Models
{
    public class Students
    {
        [Key]
        public int StudentID { get; set; }

        [ForeignKey("StudentGroups")]
        public int GroupID { get; set; }
        public virtual StudentGroups? Group { get; set; }

        [ForeignKey("EducationalOffices")]
        public int OfficeID { get; set; }
        public virtual EducationalOffices? Office { get; set; }
        
        [Required(ErrorMessage = "Поле 'Фамилия' обязательно для заполнения")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Поле 'Отчество' обязательно для заполнения")]
        public string? Patronymic { get; set; }

        public int Course {  get; set; }

        public DateTime BirthDate { get; set; }
    }
}
