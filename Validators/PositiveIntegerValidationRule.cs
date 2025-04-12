using System.Globalization;
using System.Windows.Controls;

namespace SklepInternetowyWPF.Validators
{
    public class PositiveIntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!int.TryParse(value?.ToString(), out int result) || result < 0)
                return new ValidationResult(false, "Wprowadź liczbę całkowitą większą lub równą 0.");

            return ValidationResult.ValidResult;
        }
    }
}
