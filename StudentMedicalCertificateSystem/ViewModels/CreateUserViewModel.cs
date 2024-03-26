using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class CreateUserViewModel
    {
        [Display(Name = "Имя пользователя")]
        [Required(ErrorMessage = "Необходимо ввести имя пользователя")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Необходимо ввести пароль")]
        public string Password { get; set; }
        [Display(Name = "Подтвердите пароль")]
        [Required(ErrorMessage = "Необходимо подтвердить пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Введенный пароль не совпадает")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Введите фамилию")]
        [RegularExpression(@"^[А-Яа-яA-Za-z]+$", ErrorMessage = "Поле 'Фамилия' должно содержать только буквы")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Введите имя")]
        [RegularExpression(@"^[А-Яа-яA-Za-z]+$", ErrorMessage = "Поле 'Имя' должно содержать только буквы")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Введите отчество")]
        [RegularExpression(@"^[А-Яа-яA-Za-z]+$", ErrorMessage = "Поле 'Отчество' должно содержать только буквы")]
        public string Patronymic {  get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        [Required(ErrorMessage = "Необходимо выбрать роль пользователя")]
        public string UserRole { get; set; }
        public CreateUserViewModel()
        {
            AllRoles = new List<IdentityRole>();
        }
    }
}
