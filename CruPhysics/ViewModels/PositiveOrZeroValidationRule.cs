using System.Windows.Controls;

namespace CruPhysics.ViewModels
{
    public class PositiveOrZeroValidationRule : NumberValidationRule
    {
        public PositiveOrZeroValidationRule()
        {
            Info = "������һ���Ǹ�����";
        }

        public string Info { get; set; }

        protected override ValidationResult Validate(double value)
        {
            return value < 0.0 ? new ValidationResult(false, Info) : base.Validate(value);
        }
    }
}
