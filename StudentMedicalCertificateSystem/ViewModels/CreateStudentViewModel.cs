using StudentMedicalCertificateSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class CreateStudentViewModel
    {
        [Required(ErrorMessage = "Поле 'Группа' обязательно для заполнения")]
        public int? GroupID { get; set; }
        [Required(ErrorMessage = "Поле 'Учебный офис' обязательно для заполнения")]
        public int? OfficeID { get; set; }

        [Required(ErrorMessage = "Поле 'Фамилия' обязательно для заполнения")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Поле 'Отчество' обязательно для заполнения")]
        public string? Patronymic { get; set; }
        [Range(1, 5, ErrorMessage = "Введите значение от 1 до 5")]
        public int Course { get; set; }
        [Required(ErrorMessage = "Поле 'Дата рождения' обязательно для заполнения"), DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }
}
