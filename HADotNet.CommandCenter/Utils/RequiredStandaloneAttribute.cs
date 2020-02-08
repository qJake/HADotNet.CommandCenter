using System;
using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Utils
{
    public class RequiredStandaloneAttribute : ValidationAttribute
    {       
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("HASSIO_TOKEN")))
            {
                return ValidationResult.Success;
            }

            return string.IsNullOrWhiteSpace(value?.ToString()) ? new ValidationResult(validationContext.DisplayName + " is required.") : ValidationResult.Success;
        }
    }
}