using System.Windows.Controls;

namespace CruPhysics.ViewModels
{
    public class PositiveValidationRule : NumberValidationRule
    {
        public PositiveValidationRule()
        {
            Info = "必须是一个正数！";
        }

        public string Info { get; set; }

        protected override ValidationResult Validate(double value)
        {
            return value <= 0.0 ? new ValidationResult(false, Info) : base.Validate(value);
        }
    }
}
