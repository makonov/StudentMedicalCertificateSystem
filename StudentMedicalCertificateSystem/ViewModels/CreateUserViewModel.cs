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
        public string LastName { get; set; }
        [Required(ErrorMessage = "Введите имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Введите отчество")]
        public string Patronymic {  get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        [MinLength(1, ErrorMessage = "Необходимо выбрать хотя бы одну роль")]
        public string UserRole { get; set; }
        public CreateUserViewModel()
        {
            AllRoles = new List<IdentityRole>();
        }
    }
}
