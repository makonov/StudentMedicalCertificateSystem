using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMedicalCertificateSystem.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [ForeignKey("StudentGroup")]
        public int GroupID { get; set; }
        public virtual StudentGroup? Group { get; set; }

        
        [Required(ErrorMessage = "Поле 'Фамилия' обязательно для заполнения")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Поле 'Отчество' обязательно для заполнения")]
        public string? Patronymic { get; set; }

        public int Course {  get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
