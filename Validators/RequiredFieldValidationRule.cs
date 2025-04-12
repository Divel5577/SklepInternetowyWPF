using System.Globalization;
using System.Windows.Controls;

namespace SklepInternetowyWPF.Validators
{
    public class RequiredFieldValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
                return new ValidationResult(false, "Pole jest wymagane.");

            return ValidationResult.ValidResult;
        }
    }
}
