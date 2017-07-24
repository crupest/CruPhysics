using System.Windows.Controls;

namespace CruPhysics.ViewModels
{
    public class PositiveValidationRule : NumberValidationRule
    {
        public PositiveValidationRule()
        {
            Info = "������һ��������";
        }

        public string Info { get; set; }

        protected override ValidationResult Validate(double value)
        {
            return value <= 0.0 ? new ValidationResult(false, Info) : base.Validate(value);
        }
    }
}
