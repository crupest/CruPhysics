using System;
using System.Globalization;
using System.Windows.Controls;

namespace CruPhysics.ViewModels
{
    public class NumberValidationRule : ValidationRule
    {
        protected virtual ValidationResult Validate(double value)
        {
            return new ValidationResult(true, null);
        }

        public sealed override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double result;
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                result = double.Parse(value.ToString());
            }
            catch (FormatException)
            {
                return new ValidationResult(false, "不是一个数字!");
            }
            catch (OverflowException)
            {
                return new ValidationResult(false, "超出范围！");
            }
            return Validate(result);
        }
    }
}
