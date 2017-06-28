using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CruPhysics.Controls
{
    /// <summary>
    /// Interaction logic for ObjectPropertyBox.xaml
    /// </summary>
    public partial class ObjectPropertyBox : UserControl
    {
        public static readonly DependencyProperty PropertyNameProperty;
        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty UnitProperty;
        public static readonly DependencyProperty LoseFocusTargetProperty;
        public static readonly DependencyProperty ValidationRuleProperty;

        static ObjectPropertyBox()
        {
            PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(""));
            ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(0.0));
            UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(""));
            LoseFocusTargetProperty = DependencyProperty.Register("LoseFocusTarget", typeof(UIElement), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(null));
            ValidationRuleProperty = DependencyProperty.Register("ValidationRule", typeof(NumberValidationRule), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(new NumberValidationRule(), OnValidationRulePeropertyChanged), ValidationRulePropertyValidateValue);
        }

        private static bool ValidationRulePropertyValidateValue(object value)
        {
            return value != null;
        }

        private static void OnValidationRulePeropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as ObjectPropertyBox;
            var binding = new Binding("Value")
            {
                Source = control,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                NotifyOnValidationError = true
            };
            binding.ValidationRules.Add(args.NewValue as ValidationRule);
            control.ValueTextBox.SetBinding(TextBox.TextProperty, binding);
        }

        public ObjectPropertyBox()
        {
            InitializeComponent();
        }

        public string PropertyName
        {
            get
            {
                 return (string)GetValue(PropertyNameProperty);
            }
            set
            {
                SetValue(PropertyNameProperty, value);
            }
        }

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public string Unit
        {
            get
            {
                return (string)GetValue(UnitProperty);
            }
            set
            {
                SetValue(UnitProperty, value);
            }
        }

        public UIElement LoseFocusTarget
        {
            get
            {
                return (UIElement)GetValue(LoseFocusTargetProperty);
            }
            set
            {
                SetValue(LoseFocusTargetProperty, value);
            }
        }

        public NumberValidationRule ValidationRule
        {
            get
            {
                return (NumberValidationRule)GetValue(ValidationRuleProperty);
            }
            set
            {
                SetValue(ValidationRuleProperty, value);
            }
        }

        private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                var target = LoseFocusTarget;
                if (target == null)
                {
                    target = (UIElement)Common.FindAncestor(ValueTextBox, (element) => element is UIElement && ((UIElement)element).Focusable);
                }
                if (target != null)
                    target.Focus();
            }
        }
    }
}
