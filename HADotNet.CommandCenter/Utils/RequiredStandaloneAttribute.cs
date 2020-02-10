using System;
using System.ComponentModel.DataAnnotations;

namespace HADotNet.CommandCenter.Utils
{
    public class RequiredStandaloneAttribute : ValidationAttribute
    {       
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(SupervisorEnvironment.GetSupervisorToken()))
            {
                return ValidationResult.Success;
            }

            return string.IsNullOrWhiteSpace(value?.ToString()) ? new ValidationResult(validationContext.DisplayName + " is required.") : ValidationResult.Success;
        }
    }
}