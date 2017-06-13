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

        static ObjectPropertyBox()
        {
            PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(""));
            ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(0.0));
            UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(""));
            LoseFocusTargetProperty = DependencyProperty.Register("LoseFocusTarget", typeof(UIElement), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(null));
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

        private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                var target = LoseFocusTarget;
                if (target == null)
                {
                    target = (UIElement)Common.FindAcestor(ValueTextBox, (element) => element is UIElement && ((UIElement)element).Focusable);
                }
                if (target != null)
                    target.Focus();
            }
        }
    }
}
