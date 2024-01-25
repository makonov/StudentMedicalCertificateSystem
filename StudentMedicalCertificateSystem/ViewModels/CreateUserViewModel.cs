﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class CreateUserViewModel
    {
        [Display(Name = "Имя пользователя")]
        [Required(ErrorMessage = "Необходимо ввести имя пользователя")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
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
        [Required(ErrorMessage = "Необходимо выбрать учебный офис")]
        public int OfficeID { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        [MinLength(1, ErrorMessage = "Необходимо выбрать хотя бы одну роль")]
        public IList<string> UserRoles { get; set; }
        public CreateUserViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}