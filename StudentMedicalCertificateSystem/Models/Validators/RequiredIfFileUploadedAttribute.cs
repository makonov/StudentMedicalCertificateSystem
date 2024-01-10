using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.Models.Validators
{
    public class RequiredIfFileUploadedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = (IFormFile)value;

            if (file != null && file.Length > 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "Поле обязательно для заполнения.");
        }
    }
}
