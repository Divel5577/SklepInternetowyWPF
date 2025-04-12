using System.Globalization;
using System.Windows.Controls;

namespace SklepInternetowyWPF.Validators
{
    public class PositiveDecimalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!decimal.TryParse(value?.ToString(), out decimal result) || result < 0)
                return new ValidationResult(false, "Wprowadź liczbę większą lub równą 0.");

            return ValidationResult.ValidResult;
        }
    }
}
