using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DemonstrationProject.ValidateRules
{
    public class DecimalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = value?.ToString()?.Replace(" ", "") ?? "";

            if (decimal.TryParse(input, NumberStyles.Any, cultureInfo, out _))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Введите корректную цену");
        }
    }
}
