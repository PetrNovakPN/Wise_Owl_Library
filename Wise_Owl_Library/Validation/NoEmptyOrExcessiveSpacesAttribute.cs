using System.ComponentModel.DataAnnotations;
using static System.Text.RegularExpressions.Regex;

namespace Wise_Owl_Library.Validation;

public class NoEmptyOrExcessiveSpacesAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string stringValue) return ValidationResult.Success;
        if (string.IsNullOrWhiteSpace(stringValue.Trim()))
        {
            return new ValidationResult(ErrorMessage ?? "The field cannot be empty or whitespace.");
        }

        if (IsMatch(stringValue, @"\s{2,}"))
        {
            return new ValidationResult(ErrorMessage ?? "The field cannot contain multiple consecutive spaces.");
        }

        return ValidationResult.Success;
    }
}
