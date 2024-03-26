using StudentMedicalCertificateSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class CreateStudentViewModel
    {
        [Required(ErrorMessage = "Поле 'Группа' обязательно для заполнения")]
        public int? GroupID { get; set; }

        [Required(ErrorMessage = "Поле 'Фамилия' обязательно для заполнения")]
        [RegularExpression(@"^[А-Яа-яA-Za-z]+$", ErrorMessage = "Поле 'Фамилия' должно содержать только буквы")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения")]
        [RegularExpression(@"^[А-Яа-яA-Za-z]+$", ErrorMessage = "Поле 'Имя' должно содержать только буквы")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Поле 'Отчество' обязательно для заполнения")]
        [RegularExpression(@"^[А-Яа-яA-Za-z]+$", ErrorMessage = "Поле 'Отчество' должно содержать только буквы")]
        public string? Patronymic { get; set; }
        [Range(1, 5, ErrorMessage = "Введите значение от 1 до 5")]
        public int Course { get; set; }
        [Required(ErrorMessage = "Поле 'Дата рождения' обязательно для заполнения"), DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public bool IsFromCertificate { get; set; }
    }
}
